using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mandrill.Model;

namespace Altairis.Services.Mailing.Mandrill {
    public class MandrillException : Exception {
        private static string DEFAULT_MESSAGE = "Delivery to at least one recipient was not successfull.";

        public MandrillException() : base(DEFAULT_MESSAGE) { }

        public MandrillException(IEnumerable<MandrillSendMessageResponse> results) : base(DEFAULT_MESSAGE) {
            this.Results = results;
        }

        public MandrillException(string message) : base(message) { }

        public MandrillException(string message, Exception innerException) : base(message, innerException) { }

        public IEnumerable<MandrillSendMessageResponse> Results { get; }

    }
}
