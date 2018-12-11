using System.Collections.Generic;

namespace Altairis.Services.Mailing {
    public class MailMessageDto {

        public MailAddressDto From { get; set; }

        public MailAddressDto Sender { get; set; }

        public IList<MailAddressDto> To { get; set; } = new List<MailAddressDto>();

        public IList<MailAddressDto> Cc { get; set; } = new List<MailAddressDto>();

        public IList<MailAddressDto> Bcc { get; set; } = new List<MailAddressDto>();

        public IList<MailAddressDto> ReplyTo { get; set; } = new List<MailAddressDto>();

        public IList<KeyValuePair<string, string>> CustomHeaders { get; set; } = new List<KeyValuePair<string, string>>();

        public string Subject { get; set; }

        public string BodyText { get; set; }

        public string BodyHtml { get; set; }

        public IList<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();

    }
}
