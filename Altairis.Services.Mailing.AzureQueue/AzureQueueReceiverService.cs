using System.Text.Json;
using Altairis.Services.Mailing.AzureQueue.Dto;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Altairis.Services.Mailing.AzureQueue;

public class AzureQueueReceiverService : BackgroundService, IHasAzureClients {
    internal const string MessagePrefixBlob = "blob:";
    private readonly IMailerService mailerService;
    private readonly ILogger<AzureQueueReceiverService>? logger;

    public AzureQueueReceiverService(AzureQueueMailerServiceOptions options, [FromKeyedServices(nameof(AzureQueueReceiverService))] IMailerService mailerService, ILogger<AzureQueueReceiverService>? logger) {
        if (options.QueueSasUriFactory == null && (string.IsNullOrWhiteSpace(options.ConnectionString) || string.IsNullOrWhiteSpace(options.QueueName))) throw new ArgumentException("Either queue SAS URI factory or connection string and queue name must be provided.", nameof(options));
        if (options.ContainerSasUriFactory == null && (string.IsNullOrWhiteSpace(options.ConnectionString) || string.IsNullOrWhiteSpace(options.ContainerName))) throw new ArgumentException("Either container SAS URI factory or connection string and container name must be provided.", nameof(options));

        this.ServiceOptions = options;
        this.mailerService = mailerService;
        this.logger = logger;
    }

    public AzureQueueMailerServiceOptions ServiceOptions { get; }

    public QueueClient QueueClient { get; set; } = null!;

    public BlobContainerClient ContainerClient { get; set; } = null!;

    public DateTimeOffset QueueSasRefreshTime { get; set; }

    public DateTimeOffset ContainerSasRefreshTime { get; set; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            // Refresh SAS token if needed
            await this.EnsureClientsExists();

            // Receive next message from the queue
            var qMsg = await this.QueueClient.ReceiveMessageAsync(this.ServiceOptions.QueueMessageVisibilityTimeout, stoppingToken);

            // If there is no message, wait for the next polling interval
            if (qMsg.Value == null) {
                await Task.Delay(this.ServiceOptions.QueuePollingInterval, stoppingToken);
                continue;
            }

            // Process the message
            try {
                // Validate message format
                if (!qMsg.Value.MessageText.StartsWith(MessagePrefixBlob)) throw new InvalidOperationException($"Invalid message format: {qMsg.Value.MessageText}");

                // Deserialize message from JSON
                var blobName = qMsg.Value.MessageText[MessagePrefixBlob.Length..];
                var blobClient = this.ContainerClient.GetBlobClient(blobName);
                var json = await blobClient.DownloadContentAsync(stoppingToken);
                var msg = JsonSerializer.Deserialize<QueueMailMessage>(json.Value.Content.ToString())?.ToMailMessage() ?? throw new InvalidOperationException("Failed to deserialize message.");

                // Send the message using the mailer service
                await this.mailerService.SendMessageAsync(msg);

                // Delete the message from the queue and the blob from storage
                await this.QueueClient.DeleteMessageAsync(qMsg.Value.MessageId, qMsg.Value.PopReceipt, stoppingToken);
                await blobClient.DeleteIfExistsAsync(cancellationToken: stoppingToken);

                // Log information about the processed message
                this.logger?.LogInformation("Message {MessageId} to {Recipients} with subject \"{Subject}\" processed and deleted from queue.", qMsg.Value.MessageId, string.Join(", ", msg.To.Select(x => x.Address)), msg.Subject);
            } catch (Exception ex) {
                if (qMsg.Value.DequeueCount < this.ServiceOptions.QueueMessageRetryCount) {
                    // Log warning about the failed message processing, but do not delete it from the queue, so it can be retried
                    this.logger?.LogWarning(ex, "Failed to process message {MessageId}, will retry (attempt {Attempt}/{MaxAttempts}).", qMsg.Value.MessageId, qMsg.Value.DequeueCount, this.ServiceOptions.QueueMessageRetryCount);
                } else {
                    // Log error about the failed message processing and delete it from the queue
                    this.logger?.LogError(ex, "Failed to process message {MessageId} after {MaxAttempts} attempts, deleting from queue.", qMsg.Value.MessageId, this.ServiceOptions.QueueMessageRetryCount);
                    await this.QueueClient.DeleteMessageAsync(qMsg.Value.MessageId, qMsg.Value.PopReceipt, stoppingToken);

                    // Rethrow exception if configured to do so
                    if (this.ServiceOptions.ThrowExceptionOnPoisonMessages) throw;
                }
            }
        }
    }

}
