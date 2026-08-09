using System.Net.Mail;
using System.Text;

namespace Altairis.Services.Mailing;

public static class MailMessageExtensions {

    public static void GetBodyParts(this MailMessage message, out string? bodyText, out string? bodyHtml) {
        bodyText = message.IsBodyHtml ? null : message.Body;
        bodyHtml = message.IsBodyHtml ? message.Body : null;

        foreach (var view in message.AlternateViews) {
            var mediaType = view.ContentType?.MediaType?.ToLowerInvariant();
            if (mediaType != "text/plain" && mediaType != "text/html") continue;

            var stream = view.ContentStream;
            var oldPosition = stream.CanSeek ? stream.Position : 0;
            if (stream.CanSeek) stream.Position = 0;
            using (var sr = new StreamReader(stream, Encoding.UTF8, true, leaveOpen: true)) {
                var content = sr.ReadToEnd();
                if (mediaType == "text/plain") bodyText = content;
                if (mediaType == "text/html") bodyHtml = content;
            }
            if (stream.CanSeek) stream.Position = oldPosition;
        }
    }

}
