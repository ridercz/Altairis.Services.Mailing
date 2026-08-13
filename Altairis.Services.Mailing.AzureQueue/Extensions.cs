using System.Net.Mail;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Altairis.Services.Mailing.AzureQueue.Dto;

namespace Altairis.Services.Mailing.AzureQueue;

internal static class Extensions {

    public static async Task EnsureClientsExists(this IHasAzureClients service) {
        var options = service.ServiceOptions;

        // Ensure queue client using connection string if queue SAS URI factory is not provided
        if (options.QueueSasUriFactory == null) {
            if (service.QueueClient == null) {
                if (string.IsNullOrEmpty(options.ConnectionString) || string.IsNullOrEmpty(options.QueueName)) throw new ArgumentException("Either queue SAS URI factory or connection string and queue name must be provided.", nameof(options));

                service.QueueClient = new QueueClient(options.ConnectionString, options.QueueName);
                await service.QueueClient.CreateIfNotExistsAsync();
            }
        } else if (DateTimeOffset.UtcNow >= service.QueueSasRefreshTime) {
            // Refresh queue SAS token and queue client
            var queueSasUri = await options.QueueSasUriFactory();
            var queueExpirationTime = getSasExpirationTime(queueSasUri);

            if (options.QueueSasTokenRefreshBeforeExpiration == TimeSpan.MaxValue) {
                var ttl = queueExpirationTime - DateTimeOffset.UtcNow;
                service.QueueSasRefreshTime = DateTimeOffset.UtcNow + TimeSpan.FromTicks(ttl.Ticks * 2 / 3);
            } else {
                service.QueueSasRefreshTime = queueExpirationTime - options.QueueSasTokenRefreshBeforeExpiration;
            }

            service.QueueClient = new QueueClient(queueSasUri);
        }

        // Ensure blob container client using connection string if container SAS URI factory is not provided
        if (options.ContainerSasUriFactory == null) {
            if (service.ContainerClient == null) {
                if (string.IsNullOrEmpty(options.ConnectionString) || string.IsNullOrEmpty(options.ContainerName)) throw new ArgumentException("Either container SAS URI factory or connection string and container name must be provided.", nameof(options));

                service.ContainerClient = new BlobContainerClient(options.ConnectionString, options.ContainerName);
                await service.ContainerClient.CreateIfNotExistsAsync();
            }
        } else if (DateTimeOffset.UtcNow >= service.ContainerSasRefreshTime) {
            // Refresh container SAS token and blob container client
            var containerSasUri = await options.ContainerSasUriFactory();
            var containerExpirationTime = getSasExpirationTime(containerSasUri);

            if (options.ContainerSasTokenRefreshBeforeExpiration == TimeSpan.MaxValue) {
                var ttl = containerExpirationTime - DateTimeOffset.UtcNow;
                service.ContainerSasRefreshTime = DateTimeOffset.UtcNow + TimeSpan.FromTicks(ttl.Ticks * 2 / 3);
            } else {
                service.ContainerSasRefreshTime = containerExpirationTime - options.ContainerSasTokenRefreshBeforeExpiration;
            }

            service.ContainerClient = new BlobContainerClient(containerSasUri);
        }

        static DateTimeOffset getSasExpirationTime(Uri sasUri) {
            var queryParams = System.Web.HttpUtility.ParseQueryString(sasUri.Query);
            var seParam = queryParams["se"];
            return string.IsNullOrEmpty(seParam)
                ? DateTimeOffset.MaxValue
                : DateTimeOffset.TryParse(seParam, out var expirationTime)
                ? expirationTime
                : throw new InvalidOperationException("SAS URI contains invalid expiration time.");
        }
    }

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
            To = [.. message.To.ToQueueMailAddress()],
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

        foreach (var item in message.To.ToMailAddress()) {
            msg.To.Add(item);
        }

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

}
