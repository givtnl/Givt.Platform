using Givt.Platform.Payments.Interfaces;

namespace Givt.Platform.Payments.Stripe;

internal class StripePaymentIntent : IPaymentInbound
{
    public string TransactionReference { get; }
    public string ClientToken { get; }

    public StripePaymentIntent(string transactionReference, string clientSecret)
    {
        TransactionReference = transactionReference;
        ClientToken = clientSecret;
    }
}
