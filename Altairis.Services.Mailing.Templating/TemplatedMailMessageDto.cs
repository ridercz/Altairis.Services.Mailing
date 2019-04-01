﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing.Templating {
    public class TemplatedMailMessageDto {
        public MailAddressDto From { get; set; }

        public MailAddressDto Sender { get; set; }

        public IList<MailAddressDto> To { get; set; } = new List<MailAddressDto>();

        public IList<MailAddressDto> Cc { get; set; } = new List<MailAddressDto>();

        public IList<MailAddressDto> Bcc { get; set; } = new List<MailAddressDto>();

        public IList<MailAddressDto> ReplyTo { get; set; } = new List<MailAddressDto>();

        public IList<KeyValuePair<string, string>> CustomHeaders { get; set; } = new List<KeyValuePair<string, string>>();

        public IList<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();

        public string TemplateName { get; set; }

    }
}
