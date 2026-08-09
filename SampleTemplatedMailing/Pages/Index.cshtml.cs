using Altairis.Services.Mailing.Templating;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SampleTemplatedMailing.Pages;

public class IndexModel(ITemplatedMailerService mailer) : PageModel {
    public async Task<IActionResult> OnPost() {
        // Prepare templated message
        var msg = new TemplatedMailMessageDto("Test", "to@example.com");

        // Send message with values
        await mailer.SendMessageAsync(msg, new {
            MyValue1 = 123,
            MyValue2 = "TEST",
            NullValue = (string)null
        });

        // Redirect
        return this.RedirectToPage("Sent");
    }

}
