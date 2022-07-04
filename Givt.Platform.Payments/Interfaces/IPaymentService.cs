using Givt.Platform.Payments.Enums;

namespace Givt.Platform.Payments.Interfaces;

public interface IPaymentService
{
    PaymentProvider PaymentProvider { get; }
    IPaymentCost GetInboundCost(PaymentMethod paymentMethod, decimal amount, string currency, string senderCountry, string recipientCountry);
    IPaymentCost GetOutboundCost(PaymentMethod paymentMethod, decimal amount, string currency, string senderCountry, string recipientCountry);
    Task<IPaymentInbound> CreatePaymentInboundAsync(string currency, decimal amount, decimal applicationFee, string description, string accountId, PaymentMethod paymentMethod);
    IPaymentInboundNotification GetNotification(string json);
}
