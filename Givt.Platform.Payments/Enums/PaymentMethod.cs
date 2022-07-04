namespace Givt.Platform.Payments.Enums;

public enum PaymentMethod 
{
    Bancontact = 0, // Bank redirect. BE / EUR
    Card = 1, // credit or debit. Global, 135+ currencies
    Ideal = 2, // Bank redirect. NL / EUR
    Sofort = 3, // Bank redirect. AT, BE, DE, IT, NL, ES / EUR (acquired by Klarna)
    Giropay = 4, // Bank redirect. DE / EUR
    EPS = 5, // Bank redirect. AT / EUR
    ApplePay = 6, // "global", 135+ currencies
    GooglePay = 7, // "global", 135+ currencies


    SEPADirectDebit = 10,
    BACSDirectDebit = 11,
    //CreditCard = 12,
}