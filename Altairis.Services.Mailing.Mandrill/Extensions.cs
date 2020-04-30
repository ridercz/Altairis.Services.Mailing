using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandrill.Model;

namespace Altairis.Services.Mailing.Mandrill {
    internal static class Extensions {

        public static MandrillMessage ToMandrillMessage(this MailMessageDto dto) {
            if (dto.Sender != null) throw new NotSupportedException("Sender header is not supported by Mandrill.");
            if ((dto.ReplyTo?.Count ?? 0) > 1) throw new NotSupportedException("Only one Reply-To header is supported by Mandrill.");

            // Add standard header fields
            var msg = new MandrillMessage {
                FromName = dto.From.DisplayName,
                FromEmail = dto.From.Address
            };
            if (dto.To.Any()) msg.To.AddRange(dto.To.ToMandrillAddress());
            if (dto.Cc.Any()) msg.To.AddRange(dto.To.ToMandrillAddress(MandrillMailAddressType.Cc));
            if (dto.Bcc.Any()) msg.To.AddRange(dto.To.ToMandrillAddress(MandrillMailAddressType.Bcc));

            if (dto.ReplyTo.Any()) msg.ReplyTo = dto.ReplyTo.Single().Address;
            msg.Subject = dto.Subject;

            // Add custom header fields
            foreach (var item in dto.CustomHeaders) {
                msg.Headers.Add(item.Key, item.Value);
            }

            // Construct body
            if (!string.IsNullOrWhiteSpace(dto.BodyText)) msg.Text = dto.BodyText;
            if (!string.IsNullOrWhiteSpace(dto.BodyHtml)) msg.Html = dto.BodyHtml;

            // Add attachments
            foreach (var item in dto.Attachments) {
                // Get stream as byte array
                var data = new byte[item.Stream.Length];
                item.Stream.Read(data, 0, (int)item.Stream.Length);

                // Add attachment
                msg.Attachments.Add(new MandrillAttachment { Content = data, Name = item.Name, Type = item.MimeType });
            }


            return msg;
        }

        public static List<MandrillMailAddress> ToMandrillAddress(this IEnumerable<MailAddressDto> dto, MandrillMailAddressType type = MandrillMailAddressType.To)
            => dto.Select(x => ToMandrillAddress(x, type)).ToList();


        public static MandrillMailAddress ToMandrillAddress(this MailAddressDto dto, MandrillMailAddressType type = MandrillMailAddressType.To)
            => dto == null ? null : new MandrillMailAddress {
                Email = dto.Address,
                Name = dto.DisplayName,
                Type = type
            };

    }
}
