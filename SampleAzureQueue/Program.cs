using Altairis.Services.Mailing;
using Altairis.Services.Mailing.AzureQueue;
using Altairis.Services.Mailing.SystemNetMail;

var builder = WebApplication.CreateBuilder(args);

// Register Razor Pages, so we have a way to create UI
builder.Services.AddRazorPages();

// Register configuration object for both parts of the Azure Queue mailer service (sender and receiver)
builder.Services.AddSingleton(new AzureQueueMailerServiceOptions {
    DefaultFrom = new("from@example.com", "Example From"),
    ConnectionString = builder.Configuration.GetConnectionString("MailingQueue"),
    QueueName = "mailing",
    ContainerName = "mailing",
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
app.MapStaticAssets();
app.MapRazorPages();
app.Run();
