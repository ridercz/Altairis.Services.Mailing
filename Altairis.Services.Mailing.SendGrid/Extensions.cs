using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SendGrid.Helpers.Mail;
using Altairis.Services.Mailing;

namespace Altairis.Services.Mailing.SendGrid {
    internal static class Extensions {

        public static SendGridMessage ToSendGridMessage(this MailMessageDto dto) {
            if (dto.Sender != null) throw new NotSupportedException("Sender header is not supported by SendGrid.");
            if ((dto.ReplyTo?.Count ?? 0) > 1) throw new NotSupportedException("Only one Reply-To header is supported by SendGrid.");

            var msg = new SendGridMessage();

            // Add standard header fields
            msg.From = dto.From.ToEmailAddress();
            if (dto.To.Any()) msg.AddTos(dto.To.ToEmailAddress());
            if (dto.Cc.Any()) msg.AddCcs(dto.Cc.ToEmailAddress());
            if (dto.Bcc.Any()) msg.AddBccs(dto.Bcc.ToEmailAddress());
            msg.ReplyTo = dto.ReplyTo.FirstOrDefault().ToEmailAddress();
            msg.Subject = dto.Subject;

            // Add custom header fields
            foreach (var item in dto.CustomHeaders) {
                msg.Headers.Add(item.Key, item.Value);
            }

            // Construct body
            if (!string.IsNullOrWhiteSpace(dto.BodyText)) msg.PlainTextContent = dto.BodyText;
            if (!string.IsNullOrWhiteSpace(dto.BodyHtml)) msg.HtmlContent = dto.BodyHtml;

            // Add attachments
            foreach (var item in dto.Attachments) {
                // Get stream as byte array
                var data = new byte[item.Stream.Length];
                item.Stream.Read(data, 0, (int)item.Stream.Length);

                // Base64 encode
                var encodedData = Convert.ToBase64String(data, 0, data.Length);
                msg.AddAttachment(item.Name, encodedData, item.MimeType);
            }

            return msg;
        }

        public static List<EmailAddress> ToEmailAddress(this IEnumerable<MailAddressDto> dto) {
            return dto.Select(x => ToEmailAddress((MailAddressDto)x)).ToList();
        }

        public static EmailAddress ToEmailAddress(this MailAddressDto dto) {
            if (dto == null) return null;
            return new EmailAddress(dto.Address, dto.DisplayName);
        }

    }
}