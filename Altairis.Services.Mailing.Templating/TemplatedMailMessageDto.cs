using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace Altairis.Services.Mailing.Templating {
    public class TemplatedMailMessageDto {

        public TemplatedMailMessageDto(string templateName, string recipientAddress) : this(templateName, new MailAddress(recipientAddress)) { }

        public TemplatedMailMessageDto(string templateName, MailAddress recipient = null) {
            if (templateName == null) throw new ArgumentNullException(nameof(templateName));
            if (string.IsNullOrWhiteSpace(templateName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(templateName));

            this.TemplateName = templateName;
            if (recipient != null) this.To.Add(recipient);
        }

        public MailAddress From { get; set; }

        public MailAddress Sender { get; set; }

        public IList<MailAddress> To { get; set; } = new List<MailAddress>();

        public IList<MailAddress> Cc { get; set; } = new List<MailAddress>();

        public IList<MailAddress> Bcc { get; set; } = new List<MailAddress>();

        public IList<MailAddress> ReplyTo { get; set; } = new List<MailAddress>();

        public IList<KeyValuePair<string, string>> CustomHeaders { get; set; } = new List<KeyValuePair<string, string>>();

        public IList<Attachment> Attachments { get; set; } = new List<Attachment>();

        public string TemplateName { get; set; }

    }
}
