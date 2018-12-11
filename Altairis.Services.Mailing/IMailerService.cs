using System.Threading.Tasks;

namespace Altairis.Services.Mailing {
    public interface IMailerService {

        Task SendMessageAsync(MailMessageDto message);

    }
}
