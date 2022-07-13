namespace Givt.Platform.Payments.Interfaces;

public interface IPaymentInbound
{
    string TransactionReference { get; }
    string ClientToken { get; }
}
