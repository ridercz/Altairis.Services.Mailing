using System;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing.SendGrid.IntegrationTests {
    class Program {
        private static string apiKey;

        static void Main(string[] args) {
            apiKey = args?[0] ?? throw new ArgumentException("API key missing from command line");

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
            var mx = new SendGridMailerService(apiKey);
            var msg = new MailMessage {
                From = new MailAddress("sender@rider.cz", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                Body = "Žluťoučký kůň úpěl ďábelské ódy - text."
            };
            msg.To.Add(new MailAddress("ponyboy@email.cz", "Example Recipient"));
            await mx.SendMessageAsync(msg);
        }

        private static async Task SendHtmlMail_Test() {
            var mx = new SendGridMailerService(apiKey);
            var msg = new MailMessage {
                From = new MailAddress("sender@rider.cz", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                Body = "<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>",
                IsBodyHtml = true
            };
            msg.To.Add(new MailAddress("ponyboy@email.cz", "Example Recipient"));
            await mx.SendMessageAsync(msg);
        }

        private static async Task SendAlternateMail_Test() {
            var mx = new SendGridMailerService(apiKey);
            var msg = new MailMessage {
                From = new MailAddress("sender@rider.cz", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                Body = "Žluťoučký kůň úpěl ďábelské ódy - text."
            };
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString("<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>", Encoding.UTF8, "text/html"));
            msg.To.Add(new MailAddress("ponyboy@email.cz", "Example Recipient"));
            await mx.SendMessageAsync(msg);
        }

        private static async Task SendMailWithAttachment_Test() {
            var mx = new SendGridMailerService(apiKey);
            var msg = new MailMessage {
                From = new MailAddress("sender@rider.cz", "Example Sender"),
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                Body = "Žluťoučký kůň úpěl ďábelské ódy - text."
            };
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString("<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>", Encoding.UTF8, "text/html"));
            msg.To.Add(new MailAddress("ponyboy@email.cz", "Example Recipient"));

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes("Test attachment file"))) {
                msg.Attachments.Add(new Attachment(ms, "attachment.txt", "text/plain"));
                await mx.SendMessageAsync(msg);
            }
        }

        private static async Task SendMailWithOptions_Test() {
            var options = new SendGridMailerServiceOptions {
                BodyHtmlFormat = "<html><body>{0}<hr/>This is footer</body></html>",
                BodyTextFormat = "{0}\r\n--\r\nThis is footer",
                SubjectFormat = "[test] {0}",
                DefaultFrom = new MailAddress("from@rider.cz", "Example From"),
                ApiKey = apiKey
            };

            var mx = new SendGridMailerService(options);
            var msg = new MailMessage {
                Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
                Body = "Žluťoučký kůň úpěl ďábelské ódy - text."
            };
            msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString("<p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p>", Encoding.UTF8, "text/html"));
            msg.To.Add(new MailAddress("ponyboy@email.cz", "Example Recipient"));

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes("Test attachment file"))) {
                msg.Attachments.Add(new Attachment(ms, "attachment.txt", "text/plain"));
                await mx.SendMessageAsync(msg);
            }
        }

    }
}
