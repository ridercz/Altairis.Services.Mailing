namespace Altairis.Services.Mailing.Templating;

public class ResourceTemplatedMailerServiceOptions {

    public string BodyHtmlFormatStringKeyName { get; set; } = "BodyHtmlFormatString";

    public string BodyHtmlKeyFormatString { get; set; } = "{0}_Html";

    public string BodyTextFormatStringKeyName { get; set; } = "BodyTextFormatString";

    public string BodyTextKeyFormatString { get; set; } = "{0}_Text";

    public required Type ResourceType { get; set; }

    public string SubjectFormatStringKeyName { get; set; } = "SubjectFormatString";

    public string SubjectKeyFormatString { get; set; } = "{0}_Subject";

}
