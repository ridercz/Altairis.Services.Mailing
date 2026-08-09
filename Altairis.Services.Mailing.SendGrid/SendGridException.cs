using SendGrid;

namespace Altairis.Services.Mailing.SendGrid;

public class SendGridException : Exception {
    private const string DefaultMessage = "SendGrid returned other HTTP status code than Accepted.";

    public SendGridException() : base(DefaultMessage) {
    }

    public SendGridException(string message) : base(message) {
    }

    public SendGridException(string message, Exception innerException) : base(message, innerException) {
    }

    public SendGridException(string message, Response response) : base(message) {
        this.Response = response;
    }

    public SendGridException(Response response) : base(DefaultMessage) {
        this.Response = response;
    }

    public Response Response { get; }

}
