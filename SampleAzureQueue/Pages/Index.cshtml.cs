using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Altairis.Services.Mailing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SampleAzureQueue.Pages;

public class IndexModel(IMailerService mailerService) : PageModel {

    [BindProperty, Required, EmailAddress]
    public string Recipient { get; set; } = string.Empty;

    [BindProperty, Required]
    public string Subject { get; set; } = string.Empty;

    [BindProperty, Required]
    public string Body { get; set; } = string.Empty;

    public bool MessageSent { get; private set; }

    public async Task<IActionResult> OnPostAsync() {
        if (!this.ModelState.IsValid) return this.Page();

        var message = new MailMessage {
            Subject = this.Subject,
            Body = this.Body
        };
        message.To.Add(this.Recipient);

        await mailerService.SendMessageAsync(message);

        this.MessageSent = true;
        return this.Page();
    }

}

