namespace Altairis.Services.Mailing.AzureQueue.Dto;

internal class QueueMailAddress {

    public required string Email { get; set; }

    public string? DisplayName { get; set; }

}
