namespace Altairis.Services.Mailing.Templating;

public class ResourceTemplatedMailerServiceOptions {

    public required Type ResourceType { get; set; }

    public string SubjectKeyFormatString { get; set; } = "{0}_Subject";

    public string BodyTextKeyFormatString { get; set; } = "{0}_Text";

    public string BodyHtmlKeyFormatString { get; set; } = "{0}_Html";

    public string SubjectFormatStringKeyName { get; set; } = "SubjectFormatString";

    public string BodyTextFormatStringKeyName { get; set; } = "BodyTextFormatString";

    public string BodyHtmlFormatStringKeyName { get; set; } = "BodyHtmlFormatString";

}
