using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Altairis.Services.Mailing.Rfc2822.Tests {
    public class PickupFolderMailServiceTests {

        [Fact]
        public async Task SendPlainTextMail_Test() {
            var mx = new PickupFolderMailService(CreateTempFolder("plain"));
            var msg = new MailMessageDto {
                From = new MailAddressDto("sender@example.com", "Example Sender"),
                Subject = "�lu�ou�k� k�� �p�l ��belsk� �dy - subject",
                BodyText = "�lu�ou�k� k�� �p�l ��belsk� �dy - text."
            };
            msg.To.Add(new MailAddressDto("recipient@example.com", "Example Recipient"));
            await mx.SendMessageAsync(msg);

            Assert.True(EmlFileExists(mx.PickupFolderName));
        }

        [Fact]
        public async Task SendHtmlMail_Test() {
            var mx = new PickupFolderMailService(CreateTempFolder("html"));
            var msg = new MailMessageDto {
                From = new MailAddressDto("sender@example.com", "Example Sender"),
                Subject = "�lu�ou�k� k�� �p�l ��belsk� �dy - subject",
                BodyHtml = "<html><body><p>�lu�ou�k� k�� �p�l ��belsk� �dy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDto("recipient@example.com", "Example Recipient"));
            await mx.SendMessageAsync(msg);

            Assert.True(EmlFileExists(mx.PickupFolderName));
        }

        [Fact]
        public async Task SendAlternateMail_Test() {
            var mx = new PickupFolderMailService(CreateTempFolder("alternate"));
            var msg = new MailMessageDto {
                From = new MailAddressDto("sender@example.com", "Example Sender"),
                Subject = "�lu�ou�k� k�� �p�l ��belsk� �dy - subject",
                BodyText = "�lu�ou�k� k�� �p�l ��belsk� �dy - text.",
                BodyHtml = "<html><body><p>�lu�ou�k� k�� �p�l ��belsk� �dy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDto("recipient@example.com", "Example Recipient"));
            await mx.SendMessageAsync(msg);

            Assert.True(EmlFileExists(mx.PickupFolderName));
        }

        [Fact]
        public async Task SendMailWithAttachment_Test() {
            var mx = new PickupFolderMailService(CreateTempFolder("attachment"));
            var msg = new MailMessageDto {
                From = new MailAddressDto("sender@example.com", "Example Sender"),
                Subject = "�lu�ou�k� k�� �p�l ��belsk� �dy - subject",
                BodyText = "�lu�ou�k� k�� �p�l ��belsk� �dy - text.",
                BodyHtml = "<html><body><p>�lu�ou�k� k�� �p�l ��belsk� �dy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDto("recipient@example.com", "Example Recipient"));

            using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Test attachment file"))) {
                msg.Attachments.Add(new AttachmentDto { Name = "attachment.txt", MimeType = "text/plain", Stream = ms });
                await mx.SendMessageAsync(msg);
            }

            Assert.True(EmlFileExists(mx.PickupFolderName));
        }

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
