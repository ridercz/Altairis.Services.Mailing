using System.Net.Mail;

namespace Altairis.Services.Mailing.AzureQueue;

public class AzureQueueMailerService : MailerServiceBase {

    private readonly AzureQueueMailerServiceOptions QueueOptions;

    public AzureQueueMailerService(string connectionString, string queueName) : this(new AzureQueueMailerServiceOptions {
        ConnectionString = connectionString,
        QueueName = queueName
    }) { }

    public AzureQueueMailerService(AzureQueueMailerServiceOptions options) : base(options) {
        // Validate options
        if (options.SasUriFactory == null && (string.IsNullOrWhiteSpace(options.ConnectionString) || string.IsNullOrWhiteSpace(options.QueueName))) throw new ArgumentException("Either SAS URI factory or connection string and queue name must be provided.", nameof(options));
        this.QueueOptions = options;
    }

    protected override Task SendMessageAsyncInternal(MailMessage message) {
        // Check if message does not have any attachments
        if (message.Attachments.Count > 0) throw new InvalidOperationException("Attachments are not supported by this mailer.");

        // Serialize message using MessagePack
        var messageBytes = message.ToQueueMailMessage().ToMessagePack();

        // Check if message size exceeds maximum allowed size for Azure Queue Storage (64 KB)
        if (messageBytes.Length > 64 * 1024) throw new InvalidOperationException("Message size exceeds maximum allowed size for Azure Queue Storage (64 KB).");

        // Create queue client from SAS URI or connection string
        var queueClient = this.QueueOptions.SasUriFactory != null
            ? new Azure.Storage.Queues.QueueClient(this.QueueOptions.SasUriFactory())
            : new Azure.Storage.Queues.QueueClient(this.QueueOptions.ConnectionString, this.QueueOptions.QueueName);

        // Send message to queue
        return queueClient.SendMessageAsync(Convert.ToBase64String(messageBytes), timeToLive: this.QueueOptions.MessageTtl);
    }

}
