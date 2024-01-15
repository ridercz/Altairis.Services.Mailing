using Altairis.Services.Mailing;
using Altairis.Services.Mailing.SystemNetMail;
using Altairis.Services.Mailing.Templating;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddPickupFolderMailerService(new PickupFolderMailerServiceOptions {
    PickupFolderName = @"C:\InetPub\MailRoot\pickup",
    DefaultFrom = new MailAddressDto("from@example.com")
});
builder.Services.AddResourceTemplatedMailerService(new ResourceTemplatedMailerServiceOptions {
    ResourceType = typeof(SampleTemplatedMailing.Resources.Mailer)
});

var app = builder.Build();
app.MapRazorPages();
app.Run();