using Givt.Platform.Common.Enums;
using Givt.Platform.Payments.Interfaces;

namespace Givt.Platform.Payments.Interfaces;

public interface IPaymentServiceBroker
{
    IPaymentService GetInboundPaymentService(PaymentMethod paymentMethod, decimal amount, string currency, string senderCountry, string recipientCountry);
    IPaymentService GetOutboundPaymentService(PaymentMethod paymentMethod, decimal amount, string currency, string senderCountry, string recipientCountry);
}