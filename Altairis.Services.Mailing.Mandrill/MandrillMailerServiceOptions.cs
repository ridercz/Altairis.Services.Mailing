namespace Altairis.Services.Mailing.Mandrill;

public class MandrillMailerServiceOptions : MailerServiceOptions {

    public required string ApiKey { get; set; }

    public bool TrackClicks { get; set; } = false;

    public bool TrackOpens { get; set; } = false;

    public string? TrackingDomain { get; set; }

}
