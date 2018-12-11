using System;
using System.Threading.Tasks;
using System.IO;
using System.Net.Mail;

namespace Altairis.Services.Mailing.SystemNetMail {
    public class PickupFolderMailerService : MailerServiceBase {

        public string PickupFolderName { get; }

        public PickupFolderMailerService(PickupFolderMailerServiceOptions options) : base(options) {
            if (options.PickupFolderName == null) throw new ArgumentException("Pickup folder name cannot be null.", nameof(options));
            if (string.IsNullOrWhiteSpace(options.PickupFolderName)) throw new ArgumentException("Pickup folder name cannot be empty or whitespace only string.", nameof(options));
            if (!Directory.Exists(options.PickupFolderName)) throw new DirectoryNotFoundException();

            this.PickupFolderName = options.PickupFolderName;
        }

        public PickupFolderMailerService(string pickupFolderName)
            : this(new PickupFolderMailerServiceOptions {
                PickupFolderName = pickupFolderName,
            }) { }

        protected override async Task SendMessageAsyncInternal(MailMessageDto message) {
            if (message == null) throw new ArgumentNullException(nameof(message));

            // Convert to message
            var msg = message.ToMailMessage();

            // Send using pickup folder
            using (var mx = new SmtpClient {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = this.PickupFolderName,
            }) {
                await mx.SendMailAsync(msg);
            }

        }

    }
}
