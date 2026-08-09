using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Altairis.Services.Mailing.SystemNetMail.Tests;

public class PickupFolderMailerServiceTests {

    [Fact]
    public async Task SendPlainTextMail_Test() {
        var mx = new PickupFolderMailerService(CreateTempFolder("plain"));
        var msg = new MailMessage {
            From = new MailAddress("sender@example.com", "Example Sender"),
            Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
            Body = "Žluťoučký kůň úpěl ďábelské ódy - text."
        };
        msg.To.Add(new MailAddress("recipient@example.com", "Example Recipient"));
        await mx.SendMessageAsync(msg);

        Assert.True(EmlFileExists(mx.PickupFolderName));
    }

    [Fact]
    public async Task SendHtmlMail_Test() {
        var mx = new PickupFolderMailerService(CreateTempFolder("html"));
        var msg = new MailMessage {
            From = new MailAddress("sender@example.com", "Example Sender"),
            Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
            Body = "<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>",
            IsBodyHtml = true
        };
        msg.To.Add(new MailAddress("recipient@example.com", "Example Recipient"));
        await mx.SendMessageAsync(msg);

        Assert.True(EmlFileExists(mx.PickupFolderName));
    }

    [Fact]
    public async Task SendAlternateMail_Test() {
        var mx = new PickupFolderMailerService(CreateTempFolder("alternate"));
        var msg = new MailMessage {
            From = new MailAddress("sender@example.com", "Example Sender"),
            Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
            Body = "Žluťoučký kůň úpěl ďábelské ódy - text."
        };
        msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString("<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>", Encoding.UTF8, "text/html"));
        msg.To.Add(new MailAddress("recipient@example.com", "Example Recipient"));
        await mx.SendMessageAsync(msg);

        Assert.True(EmlFileExists(mx.PickupFolderName));
    }

    [Fact]
    public async Task SendMailWithAttachment_Test() {
        var mx = new PickupFolderMailerService(CreateTempFolder("attachment"));
        var msg = new MailMessage {
            From = new MailAddress("sender@example.com", "Example Sender"),
            Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
            Body = "Žluťoučký kůň úpěl ďábelské ódy - text."
        };
        msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString("<html><body><p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p></body></html>", Encoding.UTF8, "text/html"));
        msg.To.Add(new MailAddress("recipient@example.com", "Example Recipient"));

        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes("Test attachment file"))) {
            msg.Attachments.Add(new Attachment(ms, "attachment.txt", "text/plain"));
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
            DefaultFrom = new MailAddress("from@example.com", "Example From"),
            DefaultSender = new MailAddress("sender@example.com", "Example Sender"),
            PickupFolderName = CreateTempFolder("options")
        };

        var mx = new PickupFolderMailerService(options);
        var msg = new MailMessage {
            Subject = "Žluťoučký kůň úpěl ďábelské ódy - subject",
            Body = "Žluťoučký kůň úpěl ďábelské ódy - text."
        };
        msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString("<p>Žluťoučký kůň úpěl ďábelské ódy <b>v HTML</b>.</p>", Encoding.UTF8, "text/html"));
        msg.To.Add(new MailAddress("recipient@example.com", "Example Recipient"));

        using (var ms = new MemoryStream(Encoding.UTF8.GetBytes("Test attachment file"))) {
            msg.Attachments.Add(new Attachment(ms, "attachment.txt", "text/plain"));
            await mx.SendMessageAsync(msg);
        }

        Assert.True(EmlFileExists(mx.PickupFolderName));
    }

    // Helper methods

    private static bool EmlFileExists(string folderName) {
        return Directory.EnumerateFiles(folderName, "*.eml").Count() == 1;
    }

    private static string CreateTempFolder(string suffix) {
        var folderName = Path.Combine(Path.GetTempPath(), "__TEST__SystemNetMail", DateTime.Now.ToString("yyyyMMdd-HHmmss-fffffff") + "-" + suffix);
        Directory.CreateDirectory(folderName);
        return folderName;
    }

}
