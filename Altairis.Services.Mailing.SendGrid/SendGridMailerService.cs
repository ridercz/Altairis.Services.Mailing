using System;
using System.Net.Mail;
using System.Threading.Tasks;
using SendGrid;

namespace Altairis.Services.Mailing.SendGrid {
    public class SendGridMailerService : MailerServiceBase {

        public string ApiKey { get; set; }

        public SendGridMailerService(SendGridMailerServiceOptions options) : base(options) {
            this.ApiKey = options.ApiKey;
        }

        public SendGridMailerService(string apiKey)
            : this(new SendGridMailerServiceOptions {
                ApiKey = apiKey
            }) { }

        protected override async Task SendMessageAsyncInternal(MailMessage message) {
            if (message == null) throw new ArgumentNullException(nameof(message));

            // Convert to message
            var msg = message.ToSendGridMessage();

            // Send message
            var mx = new SendGridClient(this.ApiKey);
            var response = await mx.SendEmailAsync(msg);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted) {
                throw new SendGridException($"SendGrid returned HTTP Status code {response.StatusCode}.",response);
            }
        }

    }
}
