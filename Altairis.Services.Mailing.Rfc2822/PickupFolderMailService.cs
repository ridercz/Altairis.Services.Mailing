using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace Altairis.Services.Mailing.Rfc2822 {
    public class PickupFolderMailService : MailerServiceBase {

        public Func<string> TempFileNameFactory { get; }

        public string PickupFolderName { get; }

        public PickupFolderMailService(PickupFolderMailServiceOptions options) : base(options) {
            if (options.PickupFolderName == null) throw new ArgumentException("Pickup folder name cannot be null.", nameof(options));
            if (string.IsNullOrWhiteSpace(options.PickupFolderName)) throw new ArgumentException("Pickup folder name cannot be empty or whitespace only string.", nameof(options));
            if (!Directory.Exists(options.PickupFolderName)) throw new DirectoryNotFoundException();

            this.PickupFolderName = options.PickupFolderName;
            this.TempFileNameFactory = options.TempFileNameFactory ?? Path.GetTempFileName;
        }

        public PickupFolderMailService(string pickupFolderName, Func<string> tempFileNameFactory = null)
            : this(new PickupFolderMailServiceOptions {
                PickupFolderName = pickupFolderName,
                TempFileNameFactory = tempFileNameFactory
            }) { }

        protected override async Task SendMessageAsyncInternal(MailMessageDto message) {
            if (message == null) throw new ArgumentNullException(nameof(message));

            // Convert to message
            var msg = message.ToMimeMessage();

            // Write to temp file to avoid pickup of incomplete message
            var tempFileName = this.TempFileNameFactory();
            using (var sw = File.CreateText(tempFileName)) {
                // Write envelope sender
                await sw.WriteLineAsync($"X-Sender: <{message.From.Address}>");

                // Write envelope receivers
                var receivers = message.To
                    .Union(message.Cc)
                    .Union(message.Bcc);
                foreach (var item in receivers.Select(x => x.Address)) {
                    await sw.WriteLineAsync($"X-Receiver: <{item}>");
                }

                // Flush data and write rest of message
                await sw.FlushAsync();
                msg.WriteTo(sw.BaseStream);
            }

            // Move the file to final destination
            var msgFileName = Path.Combine(this.PickupFolderName, string.Join(".", DateTime.Now.ToString("yyyyMMdd-HHmmss"), Guid.NewGuid().ToString("N"), "eml"));
            File.Move(tempFileName, msgFileName);
        }

    }
}
