using System.Net.Mail;
using System.Text;

namespace Altairis.Services.Mailing;

public abstract class MailerServiceBase : IMailerService {

    // Constructor

    protected MailerServiceBase() { }

    protected MailerServiceBase(MailerServiceOptions options) {
        ArgumentNullException.ThrowIfNull(options);
        this.DefaultFrom = options.DefaultFrom;
        this.DefaultSender = options.DefaultSender;
        this.BodyTextFormat = options.BodyTextFormat ?? "{0}";
        this.BodyHtmlFormat = options.BodyHtmlFormat ?? "<html><body>{0}</body></html>";
        this.SubjectFormat = options.SubjectFormat ?? "{0}";
    }

    // Configuration properties

    public MailAddress? DefaultFrom { get; set; }

    public MailAddress? DefaultSender { get; set; }

    public string BodyTextFormat { get; set; } = "{0}";

    public string BodyHtmlFormat { get; set; } = "<html><body>{0}</body></html>";

    public string SubjectFormat { get; set; } = "{0}";

    // Send message

    public Task SendMessageAsync(MailMessage message) {
        ArgumentNullException.ThrowIfNull(message);

        // Prepare formatted body
        message.GetBodyParts(out var bodyText, out var bodyHtml);
        bodyText = GetFormattedString(this.BodyTextFormat, bodyText ?? string.Empty);
        bodyHtml = GetFormattedString(this.BodyHtmlFormat, bodyHtml ?? string.Empty);

        // Create formatted message copy
        var newMessage = new MailMessage {
            BodyEncoding = message.BodyEncoding,
            DeliveryNotificationOptions = message.DeliveryNotificationOptions,
            From = message.From ?? this.DefaultFrom ?? throw new InvalidOperationException("From address cannot be null."),
            HeadersEncoding = message.HeadersEncoding,
            Priority = message.Priority,
            Subject = GetFormattedString(this.SubjectFormat, message.Subject),
            SubjectEncoding = message.SubjectEncoding
        };
        
        if (message.Sender != null) newMessage.Sender = message.Sender;
        message.Sender ??= this.DefaultSender;

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

    private static string GetFormattedString(string format, string s) => string.IsNullOrWhiteSpace(s) || string.IsNullOrWhiteSpace(format) ? s : string.Format(format, s);

}
