namespace Givt.Common.Models;

public class CampaignModel
{
    public Guid CampaignId { get; set; }
    public string CampaignName { get; set; }
    public string CampaignGoal { get; set; }
    public string CampaignThankYou { get; set; }
    public Guid RecipientId { get; set; }
    public string RecipientName { get; set; }
    public string RecipientCountry { get; set; }
    public string Currency { get; set; }
    public string Language { get; set; }
    public decimal FeePercentage { get; set; }
    public decimal FeeFixedAmount { get; set; }
    public IList<PaymentProviderModel> PaymentServiceProviders { get; set; }
    public GivtOfficeModel GivtOffice { get; set; }
}