using System.Net.Security;

namespace Altairis.Services.Mailing.Rfc2822 {
    public class SmtpServerMailerServiceOptions : MailerServiceOptions {

        public string HostName { get; set; }

        public int Port { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public bool AllowSsl { get; set; }

        public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; set; }

    }
}
