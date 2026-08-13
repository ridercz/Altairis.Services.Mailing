namespace Altairis.Services.Mailing.AzureQueue;

public class AzureQueueMailerServiceOptions : MailerServiceOptions {

    public string? ConnectionString { get; set; }

    public string? ContainerName { get; set; }

    public TimeSpan ContainerSasTokenRefreshBeforeExpiration { get; set; } = TimeSpan.MaxValue;

    public Func<Task<Uri>>? ContainerSasUriFactory { get; set; }

    public TimeSpan MessageTtl { get; set; } = TimeSpan.FromDays(7);

    public int QueueMessageRetryCount { get; set; } = 5;

    public TimeSpan QueueMessageVisibilityTimeout { get; set; } = TimeSpan.FromMinutes(1);

    public string? QueueName { get; set; }

    public TimeSpan QueuePollingInterval { get; set; } = TimeSpan.FromSeconds(15);

    public TimeSpan QueueSasTokenRefreshBeforeExpiration { get; set; } = TimeSpan.MaxValue;

    public Func<Task<Uri>>? QueueSasUriFactory { get; set; }

    public bool ThrowExceptionOnPoisonMessages { get; set; } = false;

}
