using Azure.Storage.Blobs;
using Azure.Storage.Queues;

namespace Altairis.Services.Mailing.AzureQueue;

internal interface IHasAzureClients {

    public AzureQueueMailerServiceOptions ServiceOptions { get; }

    public QueueClient QueueClient { get; set; }

    public BlobContainerClient ContainerClient { get; set; }

    public DateTimeOffset QueueSasRefreshTime { get; set; }

    public DateTimeOffset ContainerSasRefreshTime { get; set; }

}
