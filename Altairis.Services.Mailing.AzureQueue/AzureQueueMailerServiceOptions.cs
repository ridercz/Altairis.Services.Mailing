namespace Altairis.Services.Mailing.AzureQueue;

public class AzureQueueMailerServiceOptions : MailerServiceOptions {

    public string? ConnectionString { get; set; }

    public string? QueueName { get; set; }

    public Func<Task<Uri>>? SasUriFactory { get; set; }

    public TimeSpan SasTokenRefreshBeforeExpiration { get; set; } = TimeSpan.MaxValue;

    public TimeSpan MessageTtl { get; set; } = TimeSpan.FromDays(7);

    public TimeSpan QueuePollingInterval { get; set; } = TimeSpan.FromSeconds(15);

    public TimeSpan MessageVisibilityTimeout { get; set; } = TimeSpan.FromMinutes(1);

    public int MessageRetryCount { get; set; } = 5;

    public bool ThrowExceptionOnPoisonMessages { get; set; } = false;

}
