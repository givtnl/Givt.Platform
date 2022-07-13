using Givt.Platform.Common.Enums;

namespace Givt.Common.Models;

public class PaymentProviderModel
{
    public PaymentProvider PaymentProvider { get; set; }
    public string Reference { get; set; }
    public IList<PaymentMethod> PaymentMethods { get; set; }
    public decimal? FeePercentage { get; set; }
    public decimal? FeeFixedAmount { get; set; }

}