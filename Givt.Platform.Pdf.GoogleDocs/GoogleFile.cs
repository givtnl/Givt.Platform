using Givt.Platform.Pdf.Interfaces;

namespace Givt.Platform.Pdf.GoogleDocs;

public class GoogleFile: IFileData
{
    public byte[] Content { get; set; }
    public string Filename { get; set; }
    public string MimeType { get; set; }
}