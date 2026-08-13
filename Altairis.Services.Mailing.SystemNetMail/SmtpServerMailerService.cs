using System.Net;
using System.Net.Mail;

namespace Altairis.Services.Mailing.SystemNetMail;

public class SmtpServerMailerService(SmtpServerMailerServiceOptions options) : MailerServiceBase(options) {

    protected override async Task SendMessageAsyncInternal(MailMessage message) {
        ArgumentNullException.ThrowIfNull(message);

        // Create SMTP client and configure
        using var mx = new SmtpClient(options.HostName, options.Port) {
            DeliveryMethod = SmtpDeliveryMethod.Network,
            EnableSsl = options.EnableSsl,
            Timeout = (int)options.Timeout.TotalMilliseconds
        };

        // Set credentials if provided
        if (!string.IsNullOrEmpty(options.UserName) && !string.IsNullOrEmpty(options.Password)) mx.Credentials = new NetworkCredential(options.UserName, options.Password);

        // Send the message
        await mx.SendMailAsync(message);
    }

}
