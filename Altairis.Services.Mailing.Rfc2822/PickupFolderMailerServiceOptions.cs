using System;

namespace Altairis.Services.Mailing.Rfc2822 {
    public class PickupFolderMailerServiceOptions : MailerServiceOptions {

        public string PickupFolderName { get; set; }

        public Func<string> TempFileNameFactory { get; set; }


    }
}
