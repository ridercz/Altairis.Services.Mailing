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

            uiCulture = uiCulture ?? CultureInfo.CurrentUICulture;

            var subjectKey = string.Format(this.options.SubjectKeyFormatString, templateName);
            subjectTemplate = this.resourceManager.GetString(subjectKey, uiCulture);
            if (string.IsNullOrWhiteSpace(subjectTemplate)) throw new Exception($"Resource key {subjectKey} was not found.");

            var bodyTextKey = string.Format(this.options.BodyTextKeyFormatString, templateName, uiCulture);
            var bodyHtmlKey = string.Format(this.options.BodyHtmlKeyFormatString, templateName, uiCulture);
            bodyTextTemplate = this.resourceManager.GetString(bodyTextKey);
            bodyHtmlTemplate = this.resourceManager.GetString(bodyHtmlKey);
            if (string.IsNullOrWhiteSpace(bodyTextTemplate) && string.IsNullOrWhiteSpace(bodyHtmlTemplate)) throw new Exception($"None of {bodyTextKey} and {bodyHtmlKey} resource keys was found.");
        }

    }
}
