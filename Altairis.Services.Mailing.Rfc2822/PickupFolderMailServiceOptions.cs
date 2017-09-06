using System;
using System.Collections.Generic;
using System.Text;

namespace Altairis.Services.Mailing.Rfc2822 {
    public class PickupFolderMailServiceOptions : MailerServiceOptions {

        public string PickupFolderName { get; set; }

        public Func<string> TempFileNameFactory { get; set; }


    }
}
