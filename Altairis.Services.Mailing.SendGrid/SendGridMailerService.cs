using System.Net.Mail;
using SendGrid;

namespace Altairis.Services.Mailing.SendGrid;

public class SendGridMailerService(SendGridMailerServiceOptions options) : MailerServiceBase(options) {
    public string ApiKey { get; set; } = options.ApiKey;

    public SendGridMailerService(string apiKey)
        : this(new SendGridMailerServiceOptions {
            ApiKey = apiKey
        }) { }

    protected override async Task SendMessageAsyncInternal(MailMessage message) {
        ArgumentNullException.ThrowIfNull(message);

        // Convert to message
        var msg = message.ToSendGridMessage();

        // Send message
        var mx = new SendGridClient(this.ApiKey);
        var response = await mx.SendEmailAsync(msg);
        if (response.StatusCode != System.Net.HttpStatusCode.Accepted) {
            throw new SendGridException($"SendGrid returned HTTP Status code {response.StatusCode}.", response);
        }
    }

}
