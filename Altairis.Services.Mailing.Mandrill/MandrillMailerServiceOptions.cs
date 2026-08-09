namespace Altairis.Services.Mailing.Mandrill {
    public class MandrillMailerServiceOptions : MailerServiceOptions {
        public string ApiKey { get; set; }

        public bool TrackOpens { get; set; } = false;

        public bool TrackClicks { get; set; } = false;

        public string TrackingDomain { get; set; }
    }
}
