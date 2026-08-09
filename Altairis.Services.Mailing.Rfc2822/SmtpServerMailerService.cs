using System;
using System.Net.Mail;
using System.Net.Security;
using System.Threading.Tasks;
using MailKit.Security;

namespace Altairis.Services.Mailing.Rfc2822;

public class SmtpServerMailerService(SmtpServerMailerServiceOptions options) : MailerServiceBase(options) {
    
    public string HostName { get; } = options.HostName;

    public int Port { get; } = options.Port;

    public string UserName { get; } = options.UserName;

    public string Password { get; } = options.Password;

    public bool AllowSsl { get; } = options.AllowSsl;

    public RemoteCertificateValidationCallback ServerCertificateValidationCallback { get; } = options.ServerCertificateValidationCallback;

    protected override async Task SendMessageAsyncInternal(MailMessage message) {
        ArgumentNullException.ThrowIfNull(message);

        // Get MIME message
        var msg = message.ToMimeMessage();

        // Send message
        using var mx = new MailKit.Net.Smtp.SmtpClient();
        await mx.ConnectAsync(this.HostName, this.Port, AllowSsl ? SecureSocketOptions.Auto : SecureSocketOptions.None);
        if (this.AllowSsl) mx.ServerCertificateValidationCallback = this.ServerCertificateValidationCallback;
        if (!string.IsNullOrEmpty(this.UserName) && !string.IsNullOrEmpty(this.Password)) await mx.AuthenticateAsync(this.UserName, this.Password);
        await mx.SendAsync(msg);
        await mx.DisconnectAsync(true);
    }
}
