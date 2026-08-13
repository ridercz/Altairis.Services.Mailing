using System.Globalization;
using System.Net.Mail;
using System.Text;

namespace Altairis.Services.Mailing.Templating;

public abstract class TemplatedMailerServiceBase(IMailerService mailerService) : ITemplatedMailerService {

    public virtual Task SendMessageAsync(TemplatedMailMessageDto message, object values) => this.SendMessageAsync(message, values, CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);

    public virtual Task SendMessageAsync(TemplatedMailMessageDto message, object values, CultureInfo culture, CultureInfo uiCulture) {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(values);

        this.GetTemplates(message.TemplateName, out var subjectTemplate, out var bodyTextTemplate, out var bodyHtmlTemplate, uiCulture);
        var newMessage = ExpandTemplatedMessage(message, values, subjectTemplate, bodyTextTemplate, bodyHtmlTemplate, culture);
        return mailerService.SendMessageAsync(newMessage);
    }

    protected static MailMessage ExpandTemplatedMessage(TemplatedMailMessageDto templateMessage, object values, string subjectTemplate, string? bodyTextTemplate = null, string? bodyHtmlTemplate = null, CultureInfo? culture = null) {
        if (string.IsNullOrWhiteSpace(subjectTemplate)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(subjectTemplate));
        if (string.IsNullOrWhiteSpace(bodyTextTemplate) && string.IsNullOrWhiteSpace(bodyHtmlTemplate)) throw new ArgumentException($"At least one of {nameof(bodyTextTemplate)} and {nameof(bodyHtmlTemplate)} must be non-empty string.");

        var r = new TemplateReplacer(values, culture ?? CultureInfo.CurrentCulture);
        var bodyText = r.ReplacePlaceholders(bodyTextTemplate ?? string.Empty);
        var bodyHtml = r.ReplacePlaceholders(bodyHtmlTemplate ?? string.Empty);
        var newMessage = new MailMessage {
            Subject = r.ReplacePlaceholders(subjectTemplate),
        };
        if (templateMessage.From != null) newMessage.From = templateMessage.From;
        if (templateMessage.Sender != null) newMessage.Sender = templateMessage.Sender;
        foreach (var item in templateMessage.To) {
            newMessage.To.Add(item);
        }
        foreach (var item in templateMessage.Cc) {
            newMessage.CC.Add(item);
        }
        foreach (var item in templateMessage.Bcc) {
            newMessage.Bcc.Add(item);
        }
        foreach (var item in templateMessage.ReplyTo) {
            newMessage.ReplyToList.Add(item);
        }
        foreach (var item in templateMessage.CustomHeaders) {
            newMessage.Headers.Add(item.Key, item.Value);
        }
        foreach (var item in templateMessage.Attachments) {
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
        return newMessage;
    }

    protected abstract void GetTemplates(string templateName, out string subjectTemplate, out string bodyTextTemplate, out string bodyHtmlTemplate, CultureInfo uiCulture);

}
