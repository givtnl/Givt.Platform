using Givt.Platform.Payments.Enums;
using Givt.Platform.Payments.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Givt.Platform.Payments;

/// <summary>
/// Our definition of a Payment Service Provider. 
/// </summary>
public interface ISinglePaymentService
{
    Task<ISinglePaymentServicePaymentIntent> CreatePaymentIntent(
        string currency, 
        decimal amount, 
        decimal applicationFee, 
        string description, 
        string accountId, 
        PaymentMethod paymentMethod);
    ISinglePaymentNotification ConstructNotification(string json, IHeaderDictionary headerDictionary);
    bool CanHandle(IHeaderDictionary headerDictionary);
}