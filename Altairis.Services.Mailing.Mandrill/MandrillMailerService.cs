using System.Net.Mail;
using Mandrill;
using Mandrill.Model;

namespace Altairis.Services.Mailing.Mandrill;

public class MandrillMailerService(MandrillMailerServiceOptions options) : MailerServiceBase(options) {
    public MandrillMailerService(string apiKey) : this(new MandrillMailerServiceOptions { ApiKey = apiKey }) { }

    public string ApiKey { get; } = options.ApiKey;

    public bool TrackOpens { get; } = options.TrackOpens;

    public bool TrackClicks { get; } = options.TrackClicks;

    public string? TrackingDomain { get; } = options.TrackingDomain;

    protected override async Task SendMessageAsyncInternal(MailMessage message) {
        ArgumentNullException.ThrowIfNull(message);

        var msg = message.ToMandrillMessage();
        msg.TrackOpens = this.TrackOpens;
        msg.TrackClicks = this.TrackClicks;
        msg.TrackingDomain = this.TrackingDomain;

        using var api = new MandrillApi(this.ApiKey);
        var mx = api.Messages;
        var results = await mx.SendAsync(msg);
        var isSuccess = results.All(x => x.Status == MandrillSendMessageResponseStatus.Sent || x.Status == MandrillSendMessageResponseStatus.Queued || x.Status == MandrillSendMessageResponseStatus.Scheduled);
        if (!isSuccess) throw new MandrillException(results);
    }
}
