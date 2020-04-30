using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.Services.Mailing.Mandrill {
    public class MandrillStatus {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Status { get; set; }

        public string RejectReason { get; set; }
    }
}
