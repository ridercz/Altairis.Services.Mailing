using System.Net.Mail;
using System.Text.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;

namespace Altairis.Services.Mailing.AzureQueue;

public class AzureQueueMailerService : MailerServiceBase, IHasAzureClients {
    private readonly ILogger<AzureQueueMailerService>? Logger;

    public AzureQueueMailerService(AzureQueueMailerServiceOptions options, ILogger<AzureQueueMailerService>? logger = null) : base(options) {
        if (options.QueueSasUriFactory == null && (string.IsNullOrWhiteSpace(options.ConnectionString) || string.IsNullOrWhiteSpace(options.QueueName))) throw new ArgumentException("Either queue SAS URI factory or connection string and queue name must be provided.", nameof(options));
        if (options.ContainerSasUriFactory == null && (string.IsNullOrWhiteSpace(options.ConnectionString) || string.IsNullOrWhiteSpace(options.ContainerName))) throw new ArgumentException("Either container SAS URI factory or connection string and container name must be provided.", nameof(options));

        this.ServiceOptions = options;
        this.Logger = logger;
    }

    public AzureQueueMailerServiceOptions ServiceOptions { get; }

    public QueueClient QueueClient { get; set; } = null!;

    public BlobContainerClient ContainerClient { get; set; } = null!;

    public DateTimeOffset QueueSasRefreshTime { get; set; }

    public DateTimeOffset ContainerSasRefreshTime { get; set; }

    protected override async Task SendMessageAsyncInternal(MailMessage message) {
        // Ensure that Azure clients are initialized
        await this.EnsureClientsExists();

        // Check if message does not have any attachments
        if (message.Attachments.Count > 0) throw new InvalidOperationException("Attachments are not supported by this mailer.");

        // Serialize message to JSON using QueueMailMessage class
        var qmm = message.ToQueueMailMessage();
        var json = JsonSerializer.Serialize(qmm);

        // Save message to blob storage
        var blobName = $"{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Random.Shared.GetHexString(8, lowercase: true)}.json";
        var blobClient = this.ContainerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(new BinaryData(json), overwrite: true);
        this.Logger?.LogDebug("Message saved to blob storage: {BlobName}", blobName);

        // Send blob name to queue
        var result = await this.QueueClient.SendMessageAsync(AzureQueueReceiverService.MessagePrefixBlob + blobName);
        this.Logger?.LogInformation("Message {BlobName} to {Recipients} sent to queue: {MessageId}", blobName, string.Join(", ", qmm.To.Select(x => x.Email)), result.Value.MessageId);
    }

}
