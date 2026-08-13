namespace Altairis.Services.Mailing.AzureQueue.Dto;

internal class QueueMailMessage {

    public QueueMailAddress[] Bcc { get; set; } = [];

    public string? Body { get; set; }

    public string? BodyHtml { get; set; }

    public QueueMailAddress[] Cc { get; set; } = [];

    public required QueueMailAddress From { get; set; }

    public Dictionary<string, string> Headers { get; set; } = [];

    public QueueMailAddress[] ReplyTo { get; set; } = [];

    public QueueMailAddress? Sender { get; set; }

    public required string Subject { get; set; }

    public QueueMailAddress[] To { get; set; } = [];

}
