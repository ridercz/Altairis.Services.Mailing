﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing.SendGrid {
    public class SendGridMailerServiceOptions : MailerServiceOptions {

        public string ApiKey { get; set; }

    }
}
