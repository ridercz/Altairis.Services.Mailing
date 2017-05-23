using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing {
    public interface IMailerService {

        Task SendMessage(MailMessageDto message);

    }
}
