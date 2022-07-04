namespace Givt.Platform.Payments.Interfaces;

public interface IPaymentCost
{
    bool Supported { get; }
    string CostCurrency { get; }
    decimal? CostAmount { get; }
}