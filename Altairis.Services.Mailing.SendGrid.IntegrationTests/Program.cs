using System;
using System.IO;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing.SendGrid.IntegrationTests {
    class Program {
        private static string _apiKey;

        static void Main(string[] args) {
            _apiKey = args?[0] ?? throw new ArgumentException("API key missing from command line");

            try {
                Console.WriteLine("Sending plaintext mail...");
                SendPlainTextMail_Test().Wait();

                Console.WriteLine("Sending HTML mail...");
                SendHtmlMail_Test().Wait();

                Console.WriteLine("Sending alternate mail...");
                SendAlternateMail_Test().Wait();

                Console.WriteLine("Sending mail with attachments...");
                SendMailWithAttachment_Test().Wait();

                Console.WriteLine("Sending mail with options...");
                SendMailWithOptions_Test().Wait();

                Console.WriteLine("OK");
            }
            catch (Exception e) when (e.GetBaseException() is SendGridException) {
                var se = e.GetBaseException() as SendGridException;
                Console.WriteLine(se.Message);
                Console.WriteLine(se.Response.Headers);
                Console.WriteLine();
                Console.WriteLine(se.Response.Body.ReadAsStringAsync().Result);
            }
        }

        private static async Task SendPlainTextMail_Test() {
            var mx = new SendGridMailerService(_apiKey);
            var msg = new MailMessageDto {
                From = new MailAddressDto("sender@rider.cz", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                BodyText = "Žluťoučký kůň úpěl ďábelské ódy - text."
            };
            msg.To.Add(new MailAddressDto("ponyboy@email.cz", "Example Recipient"));
            await mx.SendMessageAsync(msg);
        }

        private static async Task SendHtmlMail_Test() {
            var mx = new SendGridMailerService(_apiKey);
            var msg = new MailMessageDto {
                From = new MailAddressDto("sender@rider.cz", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                BodyHtml = "<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDto("ponyboy@email.cz", "Example Recipient"));
            await mx.SendMessageAsync(msg);
        }

        private static async Task SendAlternateMail_Test() {
            var mx = new SendGridMailerService(_apiKey);
            var msg = new MailMessageDto {
                From = new MailAddressDto("sender@rider.cz", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                BodyText = "Žluťoučký kůň úpěl ďábelské ódy - text.",
                BodyHtml = "<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDto("ponyboy@email.cz", "Example Recipient"));
            await mx.SendMessageAsync(msg);
        }

        private static async Task SendMailWithAttachment_Test() {
            var mx = new SendGridMailerService(_apiKey);
            var msg = new MailMessageDto {
                From = new MailAddressDto("sender@rider.cz", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                BodyText = "Žluťoučký kůň úpěl ďábelské ódy - text.",
                BodyHtml = "<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>"
            };
            msg.To.Add(new MailAddressDto("ponyboy@email.cz", "Example Recipient"));

            using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Test attachment file"))) {
                msg.Attachments.Add(new AttachmentDto { Name = "attachment.txt", MimeType = "text/plain", Stream = ms });
                await mx.SendMessageAsync(msg);
            }
        }

        private static async Task SendMailWithOptions_Test() {
            var options = new SendGridMailerServiceOptions {
                BodyHtmlFormat = "<html><body>{0}<hr/>This is footer</body></html>",
                BodyTextFormat = "{0}\r\n--\r\nThis is footer",
                SubjectFormat = "[test] {0}",
                DefaultFrom = new MailAddressDto("from@rider.cz", "Example From"),
                ApiKey = _apiKey
            };

            var mx = new SendGridMailerService(options);
            var msg = new MailMessageDto {
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                BodyText = "Žluťoučký kůň úpěl ďábelské ódy - text.",
                BodyHtml = "<p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p>"
            };
            msg.To.Add(new MailAddressDto("ponyboy@email.cz", "Example Recipient"));

            using (var ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes("Test attachment file"))) {
                msg.Attachments.Add(new AttachmentDto { Name = "attachment.txt", MimeType = "text/plain", Stream = ms });
                await mx.SendMessageAsync(msg);
            }
        }

    }
}
