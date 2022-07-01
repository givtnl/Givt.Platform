namespace Givt.Platform.Pdf.GoogleDocs;

public class GoogleDocsOptions
{
    public const string SectionName = "GoogleDocs";
    public string DonationConfirmationNL { get; set; }
    public string DonationConfirmationEN { get; set; }
    public string DonationConfirmationGB { get; set; }
    public string DonationConfirmationDE { get; set; }
    public string PrivateKey { get; set; }
    public string ApplicationName { get; set; }
    public string ServiceAccountEmail { get; set; }
}