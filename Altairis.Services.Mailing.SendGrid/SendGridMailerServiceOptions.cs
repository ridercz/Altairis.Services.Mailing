namespace Altairis.Services.Mailing.SendGrid;

public class SendGridMailerServiceOptions : MailerServiceOptions {

    public required string ApiKey { get; set; }

}
