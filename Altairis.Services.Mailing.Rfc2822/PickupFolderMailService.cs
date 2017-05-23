using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

namespace Altairis.Services.Mailing.Rfc2822 {
    public class PickupFolderMailService : IMailerService {

        private readonly Func<string> getTempFileName;

        public string PickupFolderName { get; }

        public PickupFolderMailService(string pickupFolderName, Func<string> getTempFileName = null) {
            if (pickupFolderName == null) throw new ArgumentNullException(nameof(pickupFolderName));
            if (string.IsNullOrWhiteSpace(pickupFolderName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(pickupFolderName));
            if (!Directory.Exists(pickupFolderName)) throw new DirectoryNotFoundException();

            this.PickupFolderName = pickupFolderName;
            this.getTempFileName = getTempFileName ?? Path.GetTempFileName;
        }

        public async Task SendMessageAsync(MailMessageDto message) {
            if (message == null) throw new ArgumentNullException(nameof(message));

            // Convert to message
            var msg = message.ToMimeMessage();

            // Write to temp file to avoid pickup of incomplete message
            var tempFileName = this.getTempFileName();
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
