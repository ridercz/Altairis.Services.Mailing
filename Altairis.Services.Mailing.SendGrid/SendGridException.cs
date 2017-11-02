using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SendGrid;

namespace Altairis.Services.Mailing.SendGrid {
    public class SendGridException : Exception {
        private const string DEFAULT_MESSAGE = "SendGrid returned other HTTP status code than Accepted.";

        public SendGridException() : base(DEFAULT_MESSAGE) {
        }

        public SendGridException(string message) : base(message) {
        }

        public SendGridException(string message, Exception innerException) : base(message, innerException) {
        }

        public SendGridException(string message, Response response) : base(message) {
            this.Response = response;
        }

        public SendGridException(Response response) : base(DEFAULT_MESSAGE) {
            this.Response = response;
        }

        public Response Response { get; }

    }
}
