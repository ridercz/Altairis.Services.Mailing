using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.IO;
using System.Net.Mime;

namespace Altairis.Services.Mailing.SystemNetMail {
    internal static class Extensions {

        public static MailMessage ToMailMessage(this MailMessageDto dto) {
            var msg = new MailMessage();

            // Add standard header fields
            msg.From = dto.From.ToMailAddress();
            foreach (var item in dto.To) {
                msg.To.Add(item.ToMailAddress());
            }
            foreach (var item in dto.Cc) {
                msg.CC.Add(item.ToMailAddress());
            }
            foreach (var item in dto.Bcc) {
                msg.Bcc.Add(item.ToMailAddress());
            }
            foreach (var item in dto.ReplyTo) {
                msg.ReplyToList.Add(item.ToMailAddress());
            }
            msg.Sender = dto.Sender.ToMailAddress();
            msg.Subject = dto.Subject;

            // Add custom header fields
            foreach (var item in dto.CustomHeaders) {
                msg.Headers.Add(item.Key, item.Value);
            }

            // Construct body
            if (dto.BodyHtml == null && dto.BodyText != null) {
                // Plain text only
                msg.Body = dto.BodyText;
                msg.IsBodyHtml = false;
            }
            else if (dto.BodyHtml != null && dto.BodyText == null) {
                // HTML only
                msg.Body = dto.BodyHtml;
                msg.IsBodyHtml = true;
            }
            else {
                // Both
                msg.AlternateViews.Add(new AlternateView(new MemoryStream(Encoding.UTF8.GetBytes(dto.BodyText)), "text/plain;charset=utf-8") { TransferEncoding = TransferEncoding.EightBit });
                msg.AlternateViews.Add(new AlternateView(new MemoryStream(Encoding.UTF8.GetBytes(dto.BodyHtml)), "text/html;charset=utf-8") { TransferEncoding = TransferEncoding.EightBit });
            }


            // Add attachments
            foreach (var item in dto.Attachments) {
                msg.Attachments.Add(new Attachment(item.Stream, item.Name, item.MimeType));
            }

            return msg;
        }

        public static IEnumerable<MailAddress> ToMailAddress(this IEnumerable<MailAddressDto> dto) {
            return dto.Select(x => ToMailAddress((MailAddressDto)x));
        }

        public static MailAddress ToMailAddress(this MailAddressDto dto) {
            if (dto == null) return null;
            return new MailAddress(dto.Address, dto.DisplayName);
        }

    }
}