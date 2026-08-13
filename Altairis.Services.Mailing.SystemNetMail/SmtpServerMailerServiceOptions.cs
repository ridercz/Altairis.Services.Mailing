namespace Altairis.Services.Mailing.SystemNetMail;

public class SmtpServerMailerServiceOptions : MailerServiceOptions {

    public bool EnableSsl { get; set; } = false;

    public required string HostName { get; set; }

    public string? Password { get; set; }

    public int Port { get; set; } = 25;

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(100);

    public string? UserName { get; set; }

}
