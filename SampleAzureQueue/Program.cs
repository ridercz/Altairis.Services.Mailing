using Altairis.Services.Mailing;
using Altairis.Services.Mailing.AzureQueue;
using Altairis.Services.Mailing.SystemNetMail;
using Azure.Storage.Queues;

var builder = WebApplication.CreateBuilder(args);

// Register Razor Pages, so we have a way to create UI
builder.Services.AddRazorPages();

// Register configuration object for both parts of the Azure Queue mailer service (sender and receiver)
builder.Services.AddSingleton(new AzureQueueMailerServiceOptions {
    DefaultFrom = new("from@example.com", "Example From"),

    // Use either this set of options to connect to the Azure Storage using a connection string
    //ConnectionString = builder.Configuration.GetConnectionString("MailingQueue"),
    //QueueName = "mailing",
    //ContainerName = "mailing",
    
    // Or use this set of options to connect to the Azure Storage using a SAS token
    ContainerSasUriFactory = GetContainerSasToken,
    QueueSasUriFactory = GetQueueSasToken
});

// Register the Azure Queue mailer service that will send messages to the Azure Queue
builder.Services.AddSingleton<IMailerService, AzureQueueMailerService>();

// Register the Azure Queue receiver service that will receive messages from the Azure Queue
// and send them using the "inner" mailer service; It will share configuration with the Azure
// Queue mailer service, so we can use the same connection string and queue name
builder.Services.AddHostedService<AzureQueueReceiverService>();

// Register the "inner" mailer service that will be used by the receiver service to send messages
// In this case, we use a pickup folder mailer service that will write messages to a local folder
builder.Services.AddKeyedSingleton<IMailerService>(nameof(AzureQueueReceiverService), new PickupFolderMailerService(@"C:\InetPub\MailRoot\pickup"));

// Build the application, configure the request pipeline and run it
var app = builder.Build();
app.MapRazorPages();
app.Run();

// In real application, you would probably get the SAS token from a secure service, but for this sample, we will generate it on the fly
async Task<Uri> GetQueueSasToken() { 
    var queueClient = new QueueClient(builder.Configuration.GetConnectionString("MailingQueue"), "mailingsas");
    await queueClient.CreateIfNotExistsAsync();
    var sasUri = queueClient.GenerateSasUri(Azure.Storage.Sas.QueueSasPermissions.All, DateTimeOffset.UtcNow.AddSeconds(90));
    return sasUri;
}

async Task<Uri> GetContainerSasToken() {
    var containerClient = new Azure.Storage.Blobs.BlobContainerClient(builder.Configuration.GetConnectionString("MailingQueue"), "mailingsas");
    await containerClient.CreateIfNotExistsAsync();
    var sasUri = containerClient.GenerateSasUri(Azure.Storage.Sas.BlobContainerSasPermissions.All, DateTimeOffset.UtcNow.AddSeconds(90));
    return sasUri;
}