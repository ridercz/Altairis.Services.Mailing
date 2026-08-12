namespace Altairis.Services.Mailing.AzureQueue.Dto;

internal class QueueMailMessage {

    public required QueueMailAddress From { get; set; }

    public QueueMailAddress? Sender { get; set; }

    public QueueMailAddress[] ReplyTo { get; set; } = [];

    public QueueMailAddress[] Cc { get; set; } = [];

    public QueueMailAddress[] Bcc { get; set; } = [];

    public required string Subject { get; set; }

    public string? Body { get; set; } 
    
    public string? BodyHtml { get; set; }

    public Dictionary<string, string> Headers { get; set; } = [];

}
