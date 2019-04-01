using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing.Templating {
    public interface ITemplatedMailerService {

        Task SendMessageAsync(TemplatedMailMessageDto message, object values);

        Task SendMessageAsync(TemplatedMailMessageDto message, object values, CultureInfo culture, CultureInfo uiCulture);

    }
}
