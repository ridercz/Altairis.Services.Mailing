using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing.Templating {
    public abstract class TemplatedMailerServiceBase : ITemplatedMailerService {
        private readonly IMailerService mailerService;

        protected TemplatedMailerServiceBase(IMailerService mailerService) {
            this.mailerService = mailerService;
        }

        protected MailMessageDto ExpandTemplatedMessage(TemplatedMailMessageDto templateMessage, object values, string subjectTemplate, string bodyTextTemplate = null, string bodyHtmlTemplate = null, CultureInfo culture = null) {
            if (templateMessage == null) throw new ArgumentNullException(nameof(templateMessage));
            if (values == null) throw new ArgumentNullException(nameof(values));
            if (subjectTemplate == null) throw new ArgumentNullException(nameof(subjectTemplate));
            if (string.IsNullOrWhiteSpace(subjectTemplate)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(subjectTemplate));
            if (string.IsNullOrWhiteSpace(bodyTextTemplate) && string.IsNullOrWhiteSpace(bodyHtmlTemplate)) throw new ArgumentException($"At least one of {nameof(bodyTextTemplate)} and {nameof(bodyHtmlTemplate)} must be non-empty string.");

            var r = new TemplateReplacer(values, culture ?? CultureInfo.CurrentCulture);
            var newMessage = new MailMessageDto {
                Subject = r.ReplacePlaceholders(subjectTemplate),
                BodyText = r.ReplacePlaceholders(bodyTextTemplate),
                BodyHtml = r.ReplacePlaceholders(bodyHtmlTemplate),
                Attachments = templateMessage.Attachments,
                Bcc = templateMessage.Bcc,
                Cc = templateMessage.Cc,
                CustomHeaders = templateMessage.CustomHeaders,
                From = templateMessage.From,
                Sender = templateMessage.Sender,
                ReplyTo = templateMessage.ReplyTo,
                To = templateMessage.To
            };
            return newMessage;
        }

        protected abstract void GetTemplates(string templateName, out string subjectTemplate, out string bodyTextTemplate, out string bodyHtmlTemplate, CultureInfo uiCulture = null);

        public virtual Task SendMessageAsync(TemplatedMailMessageDto message, object values) => this.SendMessageAsync(message, values, CultureInfo.CurrentCulture, CultureInfo.CurrentUICulture);

        public virtual Task SendMessageAsync(TemplatedMailMessageDto message, object values, CultureInfo culture, CultureInfo uiCulture) {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (values == null) throw new ArgumentNullException(nameof(values));

            this.GetTemplates(message.TemplateName, out var subjectTemplate, out var bodyTextTemplate, out var bodyHtmlTemplate, uiCulture);
            var newMessage = this.ExpandTemplatedMessage(message, values, subjectTemplate, bodyTextTemplate, bodyHtmlTemplate, culture);
            return this.mailerService.SendMessageAsync(newMessage);
        }
    }
}
