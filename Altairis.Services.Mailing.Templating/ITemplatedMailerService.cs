using System.Globalization;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing.Templating;

public interface ITemplatedMailerService {

    Task SendMessageAsync(TemplatedMailMessageDto message, object values);

    Task SendMessageAsync(TemplatedMailMessageDto message, object values, CultureInfo culture, CultureInfo uiCulture);

}
