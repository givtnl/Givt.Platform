namespace Givt.Platform.Pdf.Interfaces;

public interface IFileData
{
    public byte[] Content { get; set; }
    public string Filename { get; set; }
    public string MimeType { get; set; }
}