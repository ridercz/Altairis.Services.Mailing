using System.IO;

namespace Altairis.Services.Mailing {
    public class AttachmentDto {
        public Stream Stream { get; set; }

        public string Name { get; set; }

        public string MimeType { get; set; }

    }
}