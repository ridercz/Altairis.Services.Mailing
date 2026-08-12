using System.Net.Mail;
using Altairis.Services.Mailing.AzureQueue.Dto;
using MessagePack;
using MessagePack.Resolvers;

namespace Altairis.Services.Mailing.AzureQueue;

internal static class Extensions {
    private static readonly MessagePackSerializerOptions MsgPackOptions = MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance).WithCompression(MessagePackCompression.Lz4BlockArray);

    // Convert MailMessage to QueueMailMessage

    public static QueueMailMessage ToQueueMailMessage(this MailMessage message) {
        if (message.From == null) throw new ArgumentException("From address must be specified.", nameof(message));

        var msg = new QueueMailMessage {
            From = message.From.ToQueueMailAddress() ?? throw new InvalidOperationException("Invalid From address."),
            Sender = message.Sender?.ToQueueMailAddress(),
            ReplyTo = [.. message.ReplyToList.ToQueueMailAddress()],
            Cc = [.. message.CC.ToQueueMailAddress()],
            Bcc = [.. message.Bcc.ToQueueMailAddress()],
            Subject = message.Subject,
        };

        foreach (var item in message.Headers.AllKeys) {
            if (item is null) throw new InvalidOperationException("Header key cannot be null.");
            msg.Headers.Add(item, message.Headers[item] ?? throw new InvalidOperationException($"Header value for key '{item}' cannot be null."));
        }

        message.GetBodyParts(out var bodyText, out var bodyHtml);
        msg.Body = bodyText;
        msg.BodyHtml = bodyHtml;

        return msg;
    }

    public static IEnumerable<QueueMailAddress> ToQueueMailAddress(this IEnumerable<MailAddress> addresses)
        => [.. addresses.Select(x => x.ToQueueMailAddress()).Where(x => x is not null).Cast<QueueMailAddress>()];

    public static QueueMailAddress? ToQueueMailAddress(this MailAddress address)
        => address is null ? null : new() {
            Email = address.Address,
            DisplayName = address.DisplayName,
        };

    // Convert QueueMailMessage to MailMessage

    public static MailMessage ToMailMessage(this QueueMailMessage message) {
        var msg = new MailMessage {
            From = message.From.ToMailAddress() ?? throw new InvalidOperationException("Invalid From address."),
            Subject = message.Subject,
        };

        if (message.Sender is not null) msg.Sender = message.Sender.ToMailAddress() ?? throw new InvalidOperationException("Invalid Sender address.");

        foreach (var item in message.ReplyTo.ToMailAddress()) {
            msg.ReplyToList.Add(item);
        }

        foreach (var item in message.Cc.ToMailAddress()) {
            msg.CC.Add(item);
        }

        foreach (var item in message.Bcc.ToMailAddress()) {
            msg.Bcc.Add(item);
        }

        foreach (var item in message.Headers) {
            msg.Headers.Add(item.Key, item.Value);
        }

        if (string.IsNullOrWhiteSpace(message.BodyHtml)) {
            msg.IsBodyHtml = false;
            msg.Body = message.Body ?? string.Empty;
        } else {
            msg.IsBodyHtml = true;
            msg.Body = message.BodyHtml;
            if (!string.IsNullOrWhiteSpace(message.Body)) msg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message.Body, null, "text/plain"));
        }
        return msg;
    }

    public static IEnumerable<MailAddress> ToMailAddress(this IEnumerable<QueueMailAddress> addresses)
        => [.. addresses.Select(x => x.ToMailAddress()).Where(x => x is not null).Cast<MailAddress>()];

    public static MailAddress? ToMailAddress(this QueueMailAddress address)
        => address is null ? null : new MailAddress(address.Email, address.DisplayName);

    // MessagePack (with LZ4) serialization helpers

    public static byte[] ToMessagePack(this QueueMailMessage message) => MessagePackSerializer.Serialize(message, MsgPackOptions);

    public static QueueMailMessage FromMessagePack(this byte[] data) => MessagePackSerializer.Deserialize<QueueMailMessage>(data, MsgPackOptions);

}
