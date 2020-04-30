using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandrill;
using Mandrill.Model;

namespace Altairis.Services.Mailing.Mandrill {
    public class MandrillMailerService : MailerServiceBase {
        public string ApiKey { get; set; }

        public MandrillMailerService(string apiKey) : this(new MandrillMailerServiceOptions { ApiKey = apiKey }) { }

        public MandrillMailerService(MandrillMailerServiceOptions options) : base(options) {
            this.ApiKey = options.ApiKey;
        }

        protected override async Task SendMessageAsyncInternal(MailMessageDto message) {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var msg = message.ToMandrillMessage();

            using (var api = new MandrillApi(this.ApiKey)) {
                var mx = api.Messages;
                var results = await mx.SendAsync(msg);
                var isSuccess = results.All(x => x.Status == MandrillSendMessageResponseStatus.Sent || x.Status == MandrillSendMessageResponseStatus.Queued || x.Status == MandrillSendMessageResponseStatus.Scheduled);
                if (!isSuccess) throw new MandrillException(results);
            }
        }
    }
}
