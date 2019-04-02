using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing.Templating {
    public class ResourceTemplatedMailerService : TemplatedMailerServiceBase {
        private readonly ResourceTemplatedMailerServiceOptions options;
        private readonly ResourceManager resourceManager;

        public ResourceTemplatedMailerService(ResourceTemplatedMailerServiceOptions options, IMailerService mailerService) : base(mailerService) {
            this.options = options;
            this.resourceManager = new ResourceManager(options.ResourceType);
        }

        protected override void GetTemplates(string templateName, out string subjectTemplate, out string bodyTextTemplate, out string bodyHtmlTemplate, CultureInfo uiCulture = null) {
            if (templateName == null) throw new ArgumentNullException(nameof(templateName));
            if (string.IsNullOrWhiteSpace(templateName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(templateName));

            // Use current UI culture if not specified
            uiCulture = uiCulture ?? CultureInfo.CurrentUICulture;

            // Read subject template
            var subjectKey = string.Format(this.options.SubjectKeyFormatString, templateName);
            subjectTemplate = this.resourceManager.GetString(subjectKey, uiCulture);
            if (string.IsNullOrWhiteSpace(subjectTemplate)) throw new Exception($"Resource key {subjectKey} was not found.");

            // Read body template
            var bodyTextKey = string.Format(this.options.BodyTextKeyFormatString, templateName, uiCulture);
            var bodyHtmlKey = string.Format(this.options.BodyHtmlKeyFormatString, templateName, uiCulture);
            bodyTextTemplate = this.resourceManager.GetString(bodyTextKey);
            bodyHtmlTemplate = this.resourceManager.GetString(bodyHtmlKey);
            if (string.IsNullOrWhiteSpace(bodyTextTemplate) && string.IsNullOrWhiteSpace(bodyHtmlTemplate)) throw new Exception($"None of {bodyTextKey} and {bodyHtmlKey} resource keys was found.");

            // Apply subject format string, if specified
            subjectTemplate = this.ApplyFormatStringIfAny(subjectTemplate, this.options.SubjectFormatStringKeyName, uiCulture);
            bodyTextTemplate = this.ApplyFormatStringIfAny(bodyTextTemplate, this.options.BodyTextFormatStringKeyName, uiCulture);
            bodyHtmlTemplate = this.ApplyFormatStringIfAny(bodyHtmlTemplate, this.options.BodyHtmlFormatStringKeyName, uiCulture);
        }

        private string ApplyFormatStringIfAny(string value, string formatStringKeyName, CultureInfo uiCulture = null) {
            if (formatStringKeyName == null) throw new ArgumentNullException(nameof(formatStringKeyName));
            if (string.IsNullOrWhiteSpace(formatStringKeyName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(formatStringKeyName));

            if (value == null) return null;

            var formatString = this.resourceManager.GetString(formatStringKeyName, uiCulture ?? CultureInfo.CurrentUICulture);
            if (string.IsNullOrWhiteSpace(formatString)) return value;
            return string.Format(formatString, value);
        }

    }
}
