using Newtonsoft.Json;

namespace Givt.Platform.Common.Exceptions;

public class NotificationException : GivtException
{
    public NotificationException(string message) : base(message)
    {
        
    }

    public override int ErrorCode => 400;
}

public class UnexpectedNotificationSignatureException : NotificationException
{
    public string ExpectedSignature { get; }
    public object IncomingData { get; }
    
    public UnexpectedNotificationSignatureException(string expectedSignature, object incomingData) : base($"Could not find signature: {expectedSignature} in notification: {JsonConvert.SerializeObject(incomingData)}")
    {
        ExpectedSignature = expectedSignature;
        IncomingData = incomingData;
        
        AdditionalInformation.Add(nameof(ExpectedSignature), expectedSignature);
        AdditionalInformation.Add(nameof(IncomingData), incomingData);
    }

    public override int ErrorCode => 418;
}