using Givt.Platform.Pdf.GoogleDocs.Helpers;
using Givt.Platform.Pdf.Interfaces;
using System.Globalization;
using System.Text;

namespace Givt.Platform.Pdf.GoogleDocs;

public class GooglePdfService : IPdfService
{
    private readonly GoogleDocsOptions _options;
    private readonly GoogleDocsService _docsService;
    private readonly GoogleDriveService _driveService;

    public GooglePdfService(GoogleDocsOptions options)
    {
        // Note on Socket resources: Google API's uses it's own IHttpClientFactory implementation with extra's. 
        _options = options;
        _docsService = new GoogleDocsService(options);
        _driveService = new GoogleDriveService(options);
    }

    public async Task<IFileData> CreateSinglePaymentReport(object report, CultureInfo cultureInfo, CancellationToken cancellationToken)
    {
        //var currSymbol = GetCurrencySymbol(report.Currency);
        // TODO: implement for real report object
        var parameters = new Dictionary<string, string>
        {
            //{"OrganisationName", report.OrganisationName},
            //{"DateGenerated", report.Timestamp.ToString(cultureInfo)},
            //{"DonationAmount", report.Amount.ToString("C", cultureInfo)},
            //{"RSIN", report.RSIN},
            //{"HmrcReference", report.HmrcReference},
            //{"CharityID", report.CharityNumber},
            //{"Last4", report.Last4},
            //{"PaymentMethod", report.PaymentMethod},
            //{"Fingerprint", report.Fingerprint},
            //{"CampaignName", report.Title},
            //{"CampaignThankYouSentence", report.ThankYou},
            //{"GivtName", report.Givt.Name},
            //{"GivtAddress", report.Givt.Address},
            //{"GivtEmail", report.Givt.Email},
            //{"GivtPhoneNumber", report.Givt.PhoneNumber},
            //{"GivtWebsite", report.Givt.Website},
        };
        // Now we only have english and netherlands without country, so I split on dash and take first element which is the language
        // I do this also for the name of the attachment
        var language = cultureInfo.TwoLetterISOLanguageName;
        var region = String.Empty;
        try { region = new RegionInfo(cultureInfo.LCID).TwoLetterISORegionName; }
        catch { /* the default language-Region is "en", this does not have a region code */ }
        var templateId = language switch
        {
            "nl" => _options.DonationConfirmationNL,
            "en" => region.ToUpper() == "GB" ? _options.DonationConfirmationGB : _options.DonationConfirmationEN,
            "de" => _options.DonationConfirmationDE,
            _ => _options.DonationConfirmationEN
        };
        var attachmentName = new StringBuilder();
        attachmentName.Append(language switch
        {
            "nl" => "ontvangstbewijs",
            "de" => "erhalt",
            _ => "receipt"
        });
        // TODO: fix this
        //attachmentName.Append(" - ").Append(report.Timestamp.ToString("yyyy-MM-dd HH-mm")).Append(".pdf");
        var document = await GenerateDocument(parameters, templateId, cancellationToken);

        return new GoogleFile()
        {
            Content = document,
            Filename = attachmentName.ToString(),
            MimeType = "application/pdf"
        };
    }

    private string GetCurrencySymbol(string ISOCurrencySymbol)
    {
        return CultureInfo
            .GetCultures(CultureTypes.AllCultures)
            .Where(c => !c.IsNeutralCulture)
            .Select(culture =>
            {
                try
                {
                    return new RegionInfo(culture.Name);
                }
                catch
                {
                    return null;
                }
            })
            .Where(ri => ri != null && ri.ISOCurrencySymbol == ISOCurrencySymbol.ToUpper())
            .Select(ri => ri.CurrencySymbol)
            .FirstOrDefault();
    }

    private async Task<byte[]> GenerateDocument(Dictionary<string, string> parameters, string documentId, CancellationToken token)
    {
        // Firstly we copy the template into a copy of the template
        var newFileId = await _driveService.CopyFile(documentId, $"tempDoc_{DateTime.UtcNow:HH:mm:ss.fff}", token);
        byte[] documentContents;
        try
        {
            // We fill in the copy so we have a filled document
            await _docsService.FillInAFile(newFileId, parameters, token);

            documentContents = _driveService.DownloadFile(newFileId);
        }
        catch (Exception)
        {
            return null;
        }
        finally
        {
            // To be sure the Drive doesn't get cluttered we remove the copied and filled document
            await _driveService.DeleteFile(newFileId, token);
        }

        return documentContents;
    }

}