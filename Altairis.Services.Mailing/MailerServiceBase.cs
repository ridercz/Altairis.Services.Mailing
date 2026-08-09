using System;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing;

public abstract class MailerServiceBase : IMailerService {

    // Constructor

    protected MailerServiceBase() { }

    protected MailerServiceBase(MailerServiceOptions options) {
        ArgumentNullException.ThrowIfNull(options);
        this.DefaultFrom = options.DefaultFrom;
        this.DefaultSender = options.DefaultSender;
        this.BodyTextFormat = options.BodyTextFormat;
        this.BodyHtmlFormat = options.BodyHtmlFormat;
        this.SubjectFormat = options.SubjectFormat;
    }

    // Configuration properties

    public MailAddress DefaultFrom { get; set; }

    public MailAddress DefaultSender { get; set; }

    public string BodyTextFormat { get; set; }

    public string BodyHtmlFormat { get; set; }

    public string SubjectFormat { get; set; }

    // Send message

    public Task SendMessageAsync(MailMessage message) {
        ArgumentNullException.ThrowIfNull(message);

        // Prepare formatted body
        message.GetBodyParts(out var bodyText, out var bodyHtml);
        bodyText = this.GetFormattedString(this.BodyTextFormat, bodyText);
        bodyHtml = this.GetFormattedString(this.BodyHtmlFormat, bodyHtml);

        // Create formatted message copy
        var newMessage = new MailMessage {
            BodyEncoding = message.BodyEncoding,
            DeliveryNotificationOptions = message.DeliveryNotificationOptions,
            From = message.From ?? this.DefaultFrom,
            HeadersEncoding = message.HeadersEncoding,
            Priority = message.Priority,
            Sender = message.Sender ?? this.DefaultSender,
            Subject = this.GetFormattedString(this.SubjectFormat, message.Subject),
            SubjectEncoding = message.SubjectEncoding
        };
        foreach (var item in message.To) {
            newMessage.To.Add(item);
        }
        foreach (var item in message.CC) {
            newMessage.CC.Add(item);
        }
        foreach (var item in message.Bcc) {
            newMessage.Bcc.Add(item);
        }
        foreach (var item in message.ReplyToList) {
            newMessage.ReplyToList.Add(item);
        }
        foreach (var item in message.Headers.AllKeys) {
            newMessage.Headers.Add(item, message.Headers[item]);
        }
        foreach (var item in message.Attachments) {
            newMessage.Attachments.Add(item);
        }

        if (!string.IsNullOrWhiteSpace(bodyText) && !string.IsNullOrWhiteSpace(bodyHtml)) {
            newMessage.Body = string.Empty;
            newMessage.IsBodyHtml = false;
            newMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyText, Encoding.UTF8, "text/plain"));
            newMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(bodyHtml, Encoding.UTF8, "text/html"));
        } else if (!string.IsNullOrWhiteSpace(bodyHtml)) {
            newMessage.Body = bodyHtml;
            newMessage.IsBodyHtml = true;
        } else {
            newMessage.Body = bodyText;
            newMessage.IsBodyHtml = false;
        }

        // Defer to actual implementation to really send message
        return this.SendMessageAsyncInternal(newMessage);
    }

    protected abstract Task SendMessageAsyncInternal(MailMessage message);

    // Helper methods

    private string GetFormattedString(string format, string s) {
        if (string.IsNullOrWhiteSpace(s)) return s;
        if (string.IsNullOrWhiteSpace(format)) return s;
        return string.Format(format, s);
    }

}
