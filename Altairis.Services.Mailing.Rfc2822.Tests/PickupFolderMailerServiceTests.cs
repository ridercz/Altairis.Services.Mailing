using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Altairis.Services.Mailing.Rfc2822.Tests {
    public class PickupFolderMailerServiceTests {

        [Fact]
        public async Task SendPlainTextMail_Test() {
            var mx = new PickupFolderMailerService(CreateTempFolder("plain"));
            var msg = new MailMessageDto {
                From = new MailAddressDto("sender@example.com", "Example Sender"),
                Subject = "luouèkı kùò úpìl ïábelské ódy - subject",
                BodyText = "luouèkı kùò úpìl ïábelské ódy - text."
            };
            msg.To.Add(new MailAddressDto("recipient@example.com", "Example Recipient"));
            await mx.SendMessageAsync(msg);

            Assert.True(EmlFileExists(mx.PickupFolderName));
        }

        [Fact]
        public async Task SendHtmlMail_Test() {
            var mx = new PickupFolderMailerService(CreateTempFolder("html"));
            var msg = new MailMessageDto {
                From = new MailAddressDto("sender@example.com", "Example Sender"),
                Subject = "luouèkı kùò úpìl ïábelské ódy - subject",
                BodyHtml = "<html><body><p>luouèkı kùò úpìl ïábelské ódy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDto("recipient@example.com", "Example Recipient"));
            await mx.SendMessageAsync(msg);

            Assert.True(EmlFileExists(mx.PickupFolderName));
        }

        [Fact]
        public async Task SendAlternateMail_Test() {
            var mx = new PickupFolderMailerService(CreateTempFolder("alternate"));
            var msg = new MailMessageDto {
                From = new MailAddressDto("sender@example.com", "Example Sender"),
                Subject = "luouèkı kùò úpìl ïábelské ódy - subject",
                BodyText = "luouèkı kùò úpìl ïábelské ódy - text.",
                BodyHtml = "<html><body><p>luouèkı kùò úpìl ïábelské ódy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDto("recipient@example.com", "Example Recipient"));
            await mx.SendMessageAsync(msg);

            Assert.True(EmlFileExists(mx.PickupFolderName));
        }

        [Fact]
        public async Task SendMailWithAttachment_Test() {
            var mx = new PickupFolderMailerService(CreateTempFolder("attachment"));
            var msg = new MailMessageDto {
                From = new MailAddressDto("sender@example.com", "Example Sender"),
                Subject = "luouèkı kùò úpìl ïábelské ódy - subject",
                BodyText = "luouèkı kùò úpìl ïábelské ódy - text.",
                BodyHtml = "<html><body><p>luouèkı kùò úpìl ïábelské ódy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDto("recipient@example.com", "Example Recipient"));

            using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Test attachment file"))) {
                msg.Attachments.Add(new AttachmentDto { Name = "attachment.txt", MimeType = "text/plain", Stream = ms });
                await mx.SendMessageAsync(msg);
            }

            Assert.True(EmlFileExists(mx.PickupFolderName));
        }

        [Fact]
        public async Task SendMailWithOptions_Test() {
            var options = new PickupFolderMailerServiceOptions {
                BodyHtmlFormat = "<html><body>{0}<hr/>This is footer</body></html>",
                BodyTextFormat = "{0}\r\n--\r\nThis is footer",
                SubjectFormat = "[test] {0}",
                DefaultFrom = new MailAddressDto("from@example.com", "Example From"),
                DefaultSender = new MailAddressDto("sender@example.com", "Example Sender"),
                PickupFolderName = CreateTempFolder("options")
            };

            var mx = new PickupFolderMailerService(options);
            var msg = new MailMessageDto {
                Subject = "luouèkı kùò úpìl ïábelské ódy - subject",
                BodyText = "luouèkı kùò úpìl ïábelské ódy - text.",
                BodyHtml = "<p>luouèkı kùò úpìl ïábelské ódy <b>v HTML</b>.</p>"
            };
            msg.To.Add(new MailAddressDto("recipient@example.com", "Example Recipient"));

            using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Test attachment file"))) {
                msg.Attachments.Add(new AttachmentDto { Name = "attachment.txt", MimeType = "text/plain", Stream = ms });
                await mx.SendMessageAsync(msg);
            }

            Assert.True(EmlFileExists(mx.PickupFolderName));
        }

        // Helper methods

        private static bool EmlFileExists(string folderName) {
            return Directory.EnumerateFiles(folderName, "*.eml").Count() == 1;
        }

        private static string CreateTempFolder(string suffix) {
            var folderName = Path.Combine(Path.GetTempPath(), "PickupFolderMailServiceTest", DateTime.Now.ToString("yyyyMMdd-HHmmss-fffffff") + "-" + suffix);
            Directory.CreateDirectory(folderName);
            return folderName;
        }

    }
}
