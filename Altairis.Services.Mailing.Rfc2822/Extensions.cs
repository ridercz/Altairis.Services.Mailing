using System.Net.Mail;
using MimeKit;

namespace Altairis.Services.Mailing.Rfc2822;

internal static class Extensions {

    public static MimeMessage ToMimeMessage(this MailMessage message) {
        var msg = new MimeMessage();

        // Add standard header fields
        if (message.From != null) msg.From.Add(message.From.ToMailboxAddress());
        msg.To.AddRange(message.To.ToMailboxAddress());
        msg.Cc.AddRange(message.CC.ToMailboxAddress());
        msg.Bcc.AddRange(message.Bcc.ToMailboxAddress());
        msg.Sender = message.Sender.ToMailboxAddress();
        msg.ReplyTo.AddRange(message.ReplyToList.ToMailboxAddress());
        msg.Subject = message.Subject;

        // Add custom header fields
        foreach (var item in message.Headers.AllKeys) {
            msg.Headers.Add(item, message.Headers[item]);
        }

        // Construct body
        message.GetBodyParts(out var bodyText, out var bodyHtml);
        var bb = new BodyBuilder {
            TextBody = bodyText,
            HtmlBody = bodyHtml
        };

        // Add attachments
        foreach (var item in message.Attachments) {
            var r = ContentType.TryParse(item.ContentType?.ToString(), out var ct);
            if (!r) ct = new ContentType("application", "octet-stream");
            if (item.ContentStream.CanSeek) item.ContentStream.Position = 0;
            bb.Attachments.Add(item.Name, item.ContentStream, ct);
        }

        msg.Body = bb.ToMessageBody();
        return msg;
    }

    public static IEnumerable<MailboxAddress> ToMailboxAddress(this IEnumerable<MailAddress> addresses) {
        return addresses.Select(ToMailboxAddress);
    }

    public static MailboxAddress ToMailboxAddress(this MailAddress address) {
        if (address == null) return null;
        return new MailboxAddress(address.DisplayName, address.Address);
    }


}
