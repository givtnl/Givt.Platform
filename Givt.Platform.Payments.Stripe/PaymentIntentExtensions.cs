using Stripe;

namespace Givt.Platform.Payments.Stripe;

public static class PaymentIntentExtensions
{
    private static ChargePaymentMethodDetails GetPaymentMethodDetails(PaymentIntent paymentIntent)
    {
        return paymentIntent.Charges?.FirstOrDefault()?.PaymentMethodDetails;
    }

    public static string GetPaymentMethod(this PaymentIntent paymentIntent)
    {
        return GetPaymentMethodDetails(paymentIntent)?.Type;
    }

    public static string GetFingerprint(this PaymentIntent paymentIntent)
    {
        var paymentMethodDetails = GetPaymentMethodDetails(paymentIntent);

        return paymentMethodDetails?.Type?.ToLower() switch
        {
            "ideal" => paymentMethodDetails.Ideal.IbanLast4,
            "bancontact" => paymentMethodDetails.Bancontact.IbanLast4,
            "sofort" => paymentMethodDetails.Sofort.IbanLast4,
            "card" => paymentMethodDetails.Card.Fingerprint,
            "googlepay" => paymentMethodDetails.Card.Fingerprint,
            "applepay" => paymentMethodDetails.Card.Fingerprint,
            _ => string.Empty
        };
    }

    public static string GetLast4(this PaymentIntent paymentIntent)
    {
        var paymentMethodDetails = GetPaymentMethodDetails(paymentIntent);

        return paymentMethodDetails?.Type?.ToLower() switch
        {
            "ideal" => paymentMethodDetails.Ideal.IbanLast4,
            "bancontact" => paymentMethodDetails.Bancontact.IbanLast4,
            "sofort" => paymentMethodDetails.Sofort.IbanLast4,
            "card" => paymentMethodDetails.Card.Last4,
            "googlepay" => paymentMethodDetails.Card.Last4,
            "applepay" => paymentMethodDetails.Card.Last4,
            _ => string.Empty
        };
    }

}