using System.Globalization;
using System.Resources;

namespace Altairis.Services.Mailing.Templating;

public class ResourceTemplatedMailerService(ResourceTemplatedMailerServiceOptions options, IMailerService mailerService) : TemplatedMailerServiceBase(mailerService) {
    private readonly ResourceManager resourceManager = new(options.ResourceType);

    protected override void GetTemplates(string templateName, out string subjectTemplate, out string bodyTextTemplate, out string bodyHtmlTemplate, CultureInfo uiCulture) {
        ArgumentNullException.ThrowIfNull(templateName);
        if (string.IsNullOrWhiteSpace(templateName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(templateName));

        // Read subject template
        var subjectKey = string.Format(options.SubjectKeyFormatString, templateName);
        subjectTemplate = this.resourceManager.GetString(subjectKey, uiCulture) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(subjectTemplate)) throw new Exception($"Resource key {subjectKey} was not found.");

        // Read body template
        var bodyTextKey = string.Format(options.BodyTextKeyFormatString, templateName);
        var bodyHtmlKey = string.Format(options.BodyHtmlKeyFormatString, templateName);
        bodyTextTemplate = this.resourceManager.GetString(bodyTextKey, uiCulture) ?? string.Empty;
        bodyHtmlTemplate = this.resourceManager.GetString(bodyHtmlKey, uiCulture) ?? string.Empty;
        if (string.IsNullOrWhiteSpace(bodyTextTemplate) && string.IsNullOrWhiteSpace(bodyHtmlTemplate)) throw new Exception($"None of {bodyTextKey} and {bodyHtmlKey} resource keys was found.");

        // Apply subject format string, if specified
        subjectTemplate = this.ApplyFormatStringIfAny(subjectTemplate, options.SubjectFormatStringKeyName, uiCulture) ?? string.Empty;
        bodyTextTemplate = this.ApplyFormatStringIfAny(bodyTextTemplate, options.BodyTextFormatStringKeyName, uiCulture) ?? string.Empty;
        bodyHtmlTemplate = this.ApplyFormatStringIfAny(bodyHtmlTemplate, options.BodyHtmlFormatStringKeyName, uiCulture) ?? string.Empty;
    }

    private string? ApplyFormatStringIfAny(string? value, string formatStringKeyName, CultureInfo uiCulture) {
        ArgumentNullException.ThrowIfNull(formatStringKeyName);
        if (string.IsNullOrWhiteSpace(formatStringKeyName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(formatStringKeyName));

        if (value == null) return null;

        var formatString = this.resourceManager.GetString(formatStringKeyName, uiCulture);
        return string.IsNullOrWhiteSpace(formatString) ? value : string.Format(formatString, value);
    }

}
