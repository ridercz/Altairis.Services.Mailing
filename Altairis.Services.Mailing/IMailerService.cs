using System.Net.Mail;

namespace Altairis.Services.Mailing;

public interface IMailerService {

    Task SendMessageAsync(MailMessage message);

}
