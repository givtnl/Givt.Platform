using Givt.Platform.Common.Enums;
using Givt.Platform.Payments.Interfaces;

namespace Givt.Platform.Payments.Impl;

public class DefaultPaymentServiceBroker : IPaymentServiceBroker
{
    private IEnumerable<IPaymentService> _services;
    public DefaultPaymentServiceBroker(IEnumerable<IPaymentService> services)
    {
        _services = services;
    }

    public IPaymentService GetInboundPaymentService(PaymentMethod paymentMethod, decimal amount, string currency, string senderCountry, string recipientCountry)
    {
        throw new NotImplementedException();
    }

    public IPaymentService GetOutboundPaymentService(PaymentMethod paymentMethod, decimal amount, string currency, string senderCountry, string recipientCountry)
    {
        throw new NotImplementedException();
    }
}