using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing {
    public abstract class MailerServiceBase : IMailerService {

        // Constructor

        protected MailerServiceBase() { }

        protected MailerServiceBase(MailerServiceOptions options) {
            if (options == null) throw new ArgumentNullException(nameof(options));
            this.DefaultFrom = options.DefaultFrom;
            this.DefaultSender = options.DefaultSender;
            this.BodyTextFormat = options.BodyTextFormat;
            this.BodyHtmlFormat = options.BodyHtmlFormat;
            this.SubjectFormat = options.SubjectFormat;
        }

        // Configuration properties

        public MailAddressDto DefaultFrom { get; set; }

        public MailAddressDto DefaultSender { get; set; }

        public string BodyTextFormat { get; set; }

        public string BodyHtmlFormat { get; set; }

        public string SubjectFormat { get; set; }

        // Send message

        public async Task SendMessageAsync(MailMessageDto message) {
            if (message == null) throw new ArgumentNullException(nameof(message));

            // Create formatted message copy
            var newMessage = new MailMessageDto {
                Bcc = message.Bcc,
                BodyHtml = this.GetFormattedString(this.BodyHtmlFormat, message.BodyHtml),
                BodyText = this.GetFormattedString(this.BodyTextFormat, message.BodyText),
                Cc = message.Cc,
                CustomHeaders = message.CustomHeaders,
                From = message.From ?? this.DefaultFrom,
                ReplyTo = message.ReplyTo,
                Sender = message.Sender ?? this.DefaultSender,
                Subject = this.GetFormattedString(this.SubjectFormat, message.Subject),
                To = message.To
            };

            // Defer to actual implementation to really send message
            await this.SendMessageAsyncInternal(newMessage);
        }

        protected abstract Task SendMessageAsyncInternal(MailMessageDto message);

        // Helper methods

        private string GetFormattedString(string format, string s) {
            if (string.IsNullOrWhiteSpace(s)) return s;
            if (string.IsNullOrWhiteSpace(format)) return s;
            return string.Format(format, s);
        }
    }
}
