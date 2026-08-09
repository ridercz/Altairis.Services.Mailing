using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Mandrill.Model;

namespace Altairis.Services.Mailing.Mandrill;

internal static class Extensions {

    public static MandrillMessage ToMandrillMessage(this MailMessage message) {
        if (message.Sender != null) throw new NotSupportedException("Sender header is not supported by Mandrill.");
        if (message.ReplyToList.Count > 1) throw new NotSupportedException("Only one Reply-To header is supported by Mandrill.");

        // Add standard header fields
        var msg = new MandrillMessage {
            FromName = message.From.DisplayName,
            FromEmail = message.From.Address
        };
        if (message.To.Any()) msg.To.AddRange(message.To.ToMandrillAddress());
        if (message.CC.Any()) msg.To.AddRange(message.CC.ToMandrillAddress(MandrillMailAddressType.Cc));
        if (message.Bcc.Any()) msg.To.AddRange(message.Bcc.ToMandrillAddress(MandrillMailAddressType.Bcc));

        if (message.ReplyToList.Any()) msg.ReplyTo = message.ReplyToList.Cast<MailAddress>().Single().Address;
        msg.Subject = message.Subject;

        // Add custom header fields
        foreach (var item in message.Headers.AllKeys) {
            msg.Headers.Add(item, message.Headers[item]);
        }

        // Construct body
        message.GetBodyParts(out var bodyText, out var bodyHtml);
        if (!string.IsNullOrWhiteSpace(bodyText)) msg.Text = bodyText;
        if (!string.IsNullOrWhiteSpace(bodyHtml)) msg.Html = bodyHtml;

        // Add attachments
        foreach (var item in message.Attachments) {
            if (item.ContentStream.CanSeek) item.ContentStream.Position = 0;
            using var ms = new MemoryStream();
            item.ContentStream.CopyTo(ms);
            msg.Attachments.Add(new() {
                Content = ms.ToArray(),
                Name = item.Name,
                Type = item.ContentType?.MediaType
            });
        }

        return msg;
    }

    public static List<MandrillMailAddress> ToMandrillAddress(this IEnumerable<MailAddress> addresses, MandrillMailAddressType type = MandrillMailAddressType.To)
        => [.. addresses.Select(x => ToMandrillAddress(x, type))];


    public static MandrillMailAddress ToMandrillAddress(this MailAddress address, MandrillMailAddressType type = MandrillMailAddressType.To)
        => address is null ? null : new() {
            Email = address.Address,
            Name = address.DisplayName,
            Type = type
        };


}
