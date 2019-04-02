using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Altairis.Services.Mailing;
using Altairis.Services.Mailing.Templating;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SampleTemplatedMailing.Pages {
    public class IndexModel : PageModel {
        private readonly ITemplatedMailerService mailer;

        public IndexModel(ITemplatedMailerService mailer) {
            this.mailer = mailer;
        }

        public void OnGet() {
        }

        public async Task<IActionResult> OnPost() {
            // Prepare templated message
            var msg = new TemplatedMailMessageDto("Test", "to@example.com");

            // Send message with values
            await this.mailer.SendMessageAsync(msg, new {
                MyValue1 = 123,
                MyValue2 = "TEST",
                NullValue = (string)null
            });

            // Redirect
            return this.RedirectToPage("Sent");
        }

    }
}