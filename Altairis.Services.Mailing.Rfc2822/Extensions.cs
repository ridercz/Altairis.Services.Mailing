﻿using System.Collections.Generic;
using System.Linq;
using MimeKit;

namespace Altairis.Services.Mailing.Rfc2822 {
    internal static class Extensions {

        public static MimeMessage ToMimeMessage(this MailMessageDto dto) {
            var msg = new MimeMessage();

            // Add standard header fields
            msg.From.Add(dto.From.ToMailboxAddress());
            msg.To.AddRange(dto.To.ToMailboxAddress());
            msg.Cc.AddRange(dto.Cc.ToMailboxAddress());
            msg.Bcc.AddRange(dto.Bcc.ToMailboxAddress());
            msg.Sender = dto.Sender.ToMailboxAddress();
            msg.ReplyTo.AddRange(dto.ReplyTo.ToMailboxAddress());
            msg.Subject = dto.Subject;

            // Add custom header fields
            foreach (var item in dto.CustomHeaders) {
                msg.Headers.Add(item.Key, item.Value);
            }

            // Construct body
            var bb = new BodyBuilder {
                TextBody = dto.BodyText,
                HtmlBody = dto.BodyHtml
            };

            // Add attachments
            foreach (var item in dto.Attachments) {
                var r = ContentType.TryParse(item.MimeType, out var ct);
                if (!r) ct = new ContentType("application", "octet-stream");
                bb.Attachments.Add(item.Name, item.Stream, ct);
            }

            msg.Body = bb.ToMessageBody();
            return msg;
        }

        public static IEnumerable<MailboxAddress> ToMailboxAddress(this IEnumerable<MailAddressDto> dto) {
            return dto.Select(x => ToMailboxAddress((MailAddressDto)x));
        }

        public static MailboxAddress ToMailboxAddress(this MailAddressDto dto) {
            if (dto == null) return null;
            return new MailboxAddress(dto.DisplayName, dto.Address);
        }

    }
}
