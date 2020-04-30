using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandrill;
using Mandrill.Model;

namespace Altairis.Services.Mailing.Mandrill {
    public class MandrillMailerService : MailerServiceBase {

        public MandrillMailerService(string apiKey) : this(new MandrillMailerServiceOptions { ApiKey = apiKey }) { }

        public MandrillMailerService(MandrillMailerServiceOptions options) : base(options) {
            this.ApiKey = options.ApiKey;
            this.TrackOpens = options.TrackOpens;
            this.TrackClicks = options.TrackClicks;
            this.TrackingDomain = options.TrackingDomain;
        }

        public string ApiKey { get; }

        public bool TrackOpens { get; }

        public bool TrackClicks { get; }

        public string TrackingDomain { get; }


        protected override async Task SendMessageAsyncInternal(MailMessageDto message) {
            if (message == null) throw new ArgumentNullException(nameof(message));

            var msg = message.ToMandrillMessage();
            msg.TrackOpens = this.TrackOpens;
            msg.TrackClicks = this.TrackClicks;
            msg.TrackingDomain = this.TrackingDomain;

            using (var api = new MandrillApi(this.ApiKey)) {
                var mx = api.Messages;
                var results = await mx.SendAsync(msg);
                var isSuccess = results.All(x => x.Status == MandrillSendMessageResponseStatus.Sent || x.Status == MandrillSendMessageResponseStatus.Queued || x.Status == MandrillSendMessageResponseStatus.Scheduled);
                if (!isSuccess) throw new MandrillException(results);
            }
        }
    }
}
