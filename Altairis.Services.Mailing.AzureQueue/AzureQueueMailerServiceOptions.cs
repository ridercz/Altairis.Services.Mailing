namespace Altairis.Services.Mailing.AzureQueue;

public class AzureQueueMailerServiceOptions : MailerServiceOptions {

    public string? ConnectionString { get; set; }

    public string? QueueName { get; set; }

    public Func<Uri>? SasUriFactory { get; set; }

    public TimeSpan MessageTtl { get; set; } = TimeSpan.FromDays(7);

}
