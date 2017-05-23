using System;
using System.Collections.Generic;
using System.Text;

namespace Altairis.Services.Mailing {
    public class MailAddressDto {

        public MailAddressDto(string address) {
            this.Address = address;
        }

        public MailAddressDto(string address, string displayName) {
            this.Address = address;
            this.DisplayName = displayName;
        }

        public string DisplayName { get; set; }

        public string Address { get; set; }

    }
}
