using System.Net.Mail;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing {
    public interface IMailerService {

        Task SendMessageAsync(MailMessage message);

    }
}
