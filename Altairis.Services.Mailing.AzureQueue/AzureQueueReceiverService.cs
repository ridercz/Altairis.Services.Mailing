using Azure.Storage.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Altairis.Services.Mailing.AzureQueue;

public class AzureQueueReceiverService(AzureQueueMailerServiceOptions options, IMailerService mailerService, ILogger<AzureQueueReceiverService>? logger) : BackgroundService {
    private DateTimeOffset sasRefreshTime;
    private QueueClient queueClient = null!;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        while (!stoppingToken.IsCancellationRequested) {
            // Refresh SAS token if needed
            await this.EnsureQueueClientExists();

            // Receive next message from the queue
            var qMsg = await this.queueClient.ReceiveMessageAsync(options.MessageVisibilityTimeout, stoppingToken);
            if (qMsg.Value != null) {
                try {
                    // Deserialize message from MessagePack
                    var msg = qMsg.Value.Body.ToArray().FromMessagePack().ToMailMessage();

                    // Send the message using the mailer service
                    await mailerService.SendMessageAsync(msg);

                    // Delete the message from the queue
                    await this.queueClient.DeleteMessageAsync(qMsg.Value.MessageId, qMsg.Value.PopReceipt, stoppingToken);

                    // Log information about the processed message
                    logger?.LogInformation("Message {MessageId} to {Recipients} with subject \"{Subject}\" processed and deleted from queue.", qMsg.Value.MessageId, string.Join(", ", msg.To.Select(x => x.Address)), msg.Subject);
                } catch (Exception ex) {
                    if(qMsg.Value.DequeueCount < options.MessageRetryCount) {
                        // Log warning about the failed message processing, but do not delete it from the queue, so it can be retried
                        logger?.LogWarning(ex, "Failed to process message {MessageId}, will retry (attempt {Attempt}/{MaxAttempts}).", qMsg.Value.MessageId, qMsg.Value.DequeueCount, options.MessageRetryCount);
                    } else {
                        // Log error about the failed message processing and delete it from the queue
                        logger?.LogError(ex, "Failed to process message {MessageId} after {MaxAttempts} attempts, deleting from queue.", qMsg.Value.MessageId, options.MessageRetryCount);
                        await this.queueClient.DeleteMessageAsync(qMsg.Value.MessageId, qMsg.Value.PopReceipt, stoppingToken);
                        
                        // Rethrow exception if configured to do so
                        if (options.ThrowExceptionOnPoisonMessages) throw; 
                    }
                }
            }

            // Wait for the next polling interval
            await Task.Delay(options.QueuePollingInterval, stoppingToken);
        }
    }

    // Helpers

    private async Task EnsureQueueClientExists() {
        // If SAS URI factory is not provided, use connection string and queue name to create the queue client if needed
        if (options.SasUriFactory == null) {
            if (this.queueClient != null) return;
            if (string.IsNullOrEmpty(options.ConnectionString) || string.IsNullOrEmpty(options.QueueName)) throw new ArgumentException("Either SAS URI factory or connection string and queue name must be provided.", nameof(options));

            this.queueClient = new QueueClient(options.ConnectionString, options.QueueName);
            await this.queueClient.CreateIfNotExistsAsync();
            logger?.LogInformation("Azure Queue Receiver Service initialized with connection string, using queue {QueueName}.", this.queueClient.Uri);
            return;
        }

        // If SAS URI factory is provided, check if the SAS token needs to be refreshed
        if (DateTimeOffset.UtcNow < this.sasRefreshTime) return;

        // Get new SAS URI and find out its expiration time
        var sasUri = await options.SasUriFactory();
        var expirationTime = GetSasExpirationTime(sasUri);

        // Calculate new SAS refresh time
        if (options.SasTokenRefreshBeforeExpiration == TimeSpan.MaxValue) {
            // Refresh SAS token at two thirds of its lifetime
            var ttl = expirationTime - DateTimeOffset.UtcNow;
            this.sasRefreshTime = DateTimeOffset.UtcNow + TimeSpan.FromTicks(ttl.Ticks * 2 / 3);
        } else {
            // Use fixed refresh time before expiration
            this.sasRefreshTime = expirationTime - options.SasTokenRefreshBeforeExpiration;
        }

        // Update queue client with new SAS URI
        this.queueClient = new QueueClient(sasUri);

        // Log information about the new SAS token and refresh time
        logger?.LogInformation("SAS token refreshed, new expiration time: {ExpirationTime}, next refresh time: {RefreshTime}.", expirationTime, this.sasRefreshTime);
    }

    private static DateTimeOffset GetSasExpirationTime(Uri sasUri) {
        var queryParams = System.Web.HttpUtility.ParseQueryString(sasUri.Query);
        var seParam = queryParams["se"];
        return string.IsNullOrEmpty(seParam)
            ? DateTimeOffset.MaxValue // If no expiration time is specified, assume the SAS URI never expires
            : DateTimeOffset.TryParse(seParam, out var expirationTime)
            ? expirationTime
            : throw new InvalidOperationException("SAS URI contains invalid expiration time.");
    }

}
