using System;
using System.Collections.Generic;
using System.Text;

namespace Altairis.Services.Mailing {
    public class MailerServiceOptions {

        public MailAddressDto DefaultFrom { get; set; }

        public MailAddressDto DefaultSender { get; set; }

        public string BodyTextFormat { get; set; }

        public string BodyHtmlFormat { get; set; }

        public string SubjectFormat { get; set; }

    }
}
