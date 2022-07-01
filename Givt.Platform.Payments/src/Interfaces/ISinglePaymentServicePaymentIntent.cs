namespace Givt.Platform.Payments.Interfaces;

public interface ISinglePaymentServicePaymentIntent
{
    string TransactionReference{get;}
    string ClientToken { get; }
}
