using System.Net.Mail;
using MimeKit;
using MimeKit.Encodings;

namespace Altairis.Services.Mailing.Rfc2822;

internal static class Extensions {

    public static MimeMessage ToMimeMessage(this MailMessage message) {
        var msg = new MimeMessage();

        // Add standard header fields
        if (message.From != null) msg.From.Add(message.From.ToMailboxAddress() ?? throw new InvalidOperationException("Invalid From address."));
        msg.To.AddRange(message.To.ToMailboxAddress());
        msg.Cc.AddRange(message.CC.ToMailboxAddress());
        msg.Bcc.AddRange(message.Bcc.ToMailboxAddress());
        msg.Sender = message.Sender?.ToMailboxAddress();
        msg.ReplyTo.AddRange(message.ReplyToList.ToMailboxAddress());
        msg.Subject = message.Subject;

        // Add custom header fields
        foreach (var item in message.Headers.AllKeys) {
            if (item is null) throw new InvalidOperationException("Header key cannot be null.");
            msg.Headers.Add(item, message.Headers[item] ?? throw new InvalidOperationException($"Header value for key '{item}' cannot be null."));
        }

        // Construct body
        message.GetBodyParts(out var bodyText, out var bodyHtml);
        var bb = new BodyBuilder {
            TextBody = bodyText,
            HtmlBody = bodyHtml
        };

        // Add attachments
        foreach (var item in message.Attachments) {
            _ = ContentType.TryParse(item.ContentType?.ToString(), out var ct);
            ct ??= new ContentType("application", "octet-stream");
            if (item.ContentStream.CanSeek) item.ContentStream.Position = 0;
            var me = new MimePart(ct) {
                FileName = item.Name,
                Content = new MimeContent(item.ContentStream),
            };
            bb.Attachments.Add(me);
        }

        msg.Body = bb.ToMessageBody();
        return msg;
    }

    public static IEnumerable<MailboxAddress> ToMailboxAddress(this IEnumerable<MailAddress> addresses) => addresses.Select(x => x.ToMailboxAddress()).Where(x => x is not null)!;

    public static MailboxAddress? ToMailboxAddress(this MailAddress address) {
        return address == null ? null : new MailboxAddress(address.DisplayName, address.Address);
    }


}
