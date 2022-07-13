using Givt.Platform.Payments.Interfaces;
using Stripe;
using PaymentMethod = Givt.Platform.Common.Enums.PaymentMethod;

namespace Givt.Platform.Payments.Stripe;

internal class StripePaymentNotification : IInboundPaymentNotification
{
    private readonly Event stripeEvent;
    private readonly PaymentIntent paymentIntent;

    public StripePaymentNotification(Event stripeEvent)
    {
        // Stripe json deserialisation has constructed both Event and inner Event.Data.Object,
        // so we can just cast the Data.Object to a PaymentIntent
        this.stripeEvent = stripeEvent;
        paymentIntent = stripeEvent.Data.Object as PaymentIntent;
    }

    #region Properties
    public string TransactionReference => paymentIntent?.Id;
    // Get the transaction datetime, fallback to the datetime of the creation of the payment intent. 
    public DateTime? TransactionDate { get => paymentIntent.Charges?.First()?.Created ?? paymentIntent.Created; }
    public PaymentMethod PaymentMethod => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), paymentIntent.GetPaymentMethod(), true);
    public string Last4 => paymentIntent.GetLast4();
    public string Fingerprint => paymentIntent.GetFingerprint();

    #endregion

    #region Status

    public bool Processing => stripeEvent?.Type == Events.PaymentIntentProcessing;
    public bool Succeeded => stripeEvent?.Type == Events.PaymentIntentSucceeded;
    public bool Cancelled => stripeEvent?.Type == Events.PaymentIntentCanceled;
    public bool Failed => stripeEvent?.Type == Events.PaymentIntentPaymentFailed;

    #endregion

}