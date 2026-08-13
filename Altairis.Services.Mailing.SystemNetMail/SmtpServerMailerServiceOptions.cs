using System.Net.Security;

namespace Altairis.Services.Mailing.SystemNetMail;

public class SmtpServerMailerServiceOptions : MailerServiceOptions {

    public required string HostName { get; set; }

    public int Port { get; set; } = 25;

    public string? UserName { get; set; }

    public string? Password { get; set; }

    public bool EnableSsl { get; set; } = false;

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);

}
