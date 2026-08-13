namespace Altairis.Services.Mailing.AzureQueue.Dto;

internal class QueueMailAddress {

    public string? DisplayName { get; set; }

    public required string Email { get; set; }

}
