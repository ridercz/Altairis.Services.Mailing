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

        public async Task OnPost() {
            var msg = new TemplatedMailMessageDto { TemplateName = "Test" };
            msg.From = new MailAddressDto("from@example.com");
            msg.To.Add(new MailAddressDto("to@example.com"));
            await this.mailer.SendMessageAsync(msg, new { MyValue = 123 });
        }

    }
}