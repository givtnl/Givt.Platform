namespace Givt.Platform.Payments.Stripe;

public class StripeOptions
{
    public static string SectionName = "Stripe";

    public string StripeApiKey { get; set; }
    public string EndpointSecret { get; set; }
}
