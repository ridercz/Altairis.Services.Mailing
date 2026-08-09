using System.Net.Mail;
using SendGrid.Helpers.Mail;

namespace Altairis.Services.Mailing.SendGrid;

internal static class Extensions {

    public static SendGridMessage ToSendGridMessage(this MailMessage message) {
        if (message.Sender != null) throw new NotSupportedException("Sender header is not supported by SendGrid.");
        if (message.ReplyToList.Count > 1) throw new NotSupportedException("Only one Reply-To header is supported by SendGrid.");
        if (message.From == null) throw new ArgumentException("From address must be specified.", nameof(message));

        // Add standard header fields
        var msg = new SendGridMessage {
            From = message.From.ToEmailAddress()
        };
        if (message.To.Any()) msg.AddTos(message.To.ToEmailAddress());
        if (message.CC.Any()) msg.AddCcs(message.CC.ToEmailAddress());
        if (message.Bcc.Any()) msg.AddBccs(message.Bcc.ToEmailAddress());
        msg.ReplyTo = message.ReplyToList.Cast<MailAddress>().FirstOrDefault()?.ToEmailAddress();
        msg.Subject = message.Subject;

        // Add custom header fields
        foreach (var item in message.Headers.AllKeys) {
            if (item is null) throw new InvalidOperationException("Header key cannot be null.");
            msg.Headers.Add(item, message.Headers[item] ?? throw new InvalidOperationException($"Header value for key '{item}' cannot be null."));
        }

        // Construct body
        message.GetBodyParts(out var bodyText, out var bodyHtml);
        if (!string.IsNullOrWhiteSpace(bodyText)) msg.PlainTextContent = bodyText;
        if (!string.IsNullOrWhiteSpace(bodyHtml)) msg.HtmlContent = bodyHtml;

        // Add attachments
        foreach (var item in message.Attachments) {
            if (item.ContentStream.CanSeek) item.ContentStream.Position = 0;
            using var ms = new MemoryStream();
            item.ContentStream.CopyTo(ms);
            var encodedData = Convert.ToBase64String(ms.ToArray());
            msg.AddAttachment(item.Name, encodedData, item.ContentType?.MediaType);
        }

        return msg;
    }

    public static List<EmailAddress> ToEmailAddress(this IEnumerable<MailAddress> addresses) => [.. addresses.Select(x => x.ToEmailAddress()).Where(x => x is not null).Cast<EmailAddress>()];

    public static EmailAddress? ToEmailAddress(this MailAddress address) {
        return address == null ? null : new EmailAddress(address.Address, address.DisplayName);
    }


}
