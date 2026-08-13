using System.Net.Security;

namespace Altairis.Services.Mailing.Rfc2822;

public class SmtpServerMailerServiceOptions : MailerServiceOptions {

    public bool AllowSsl { get; set; }

    public required string HostName { get; set; }

    public string? Password { get; set; }

    public int Port { get; set; }

    public RemoteCertificateValidationCallback? ServerCertificateValidationCallback { get; set; }

    public string? UserName { get; set; }

}
