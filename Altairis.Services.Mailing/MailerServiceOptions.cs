using System.Net.Mail;

namespace Altairis.Services.Mailing;

public class MailerServiceOptions {

    public MailAddress? DefaultFrom { get; set; }

    public MailAddress? DefaultSender { get; set; }

    public string? BodyTextFormat { get; set; }

    public string? BodyHtmlFormat { get; set; }

    public string? SubjectFormat { get; set; }

}
