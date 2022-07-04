namespace Givt.Platform.Email.Postmark;

public class PostmarkOptions
{
    public const string SectionName = "Postmark";

    public string ApiKey { get; set; }
    public string SupportAddress { get; set; }
    public string SupportName { get; set; }
    public string EnvironmentName { get; set; } = "Production";

    public string MailReportSingleDonationTemplate { get; set; }
}
