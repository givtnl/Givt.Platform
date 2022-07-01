using Givt.Platform.Common.Enums;

namespace Givt.Platform.Common.Exceptions;

public abstract class GivtException : Exception
{
    public Dictionary<string, object> AdditionalInformation { get; }

    public abstract int ErrorCode { get; }
    public GivtExceptionLevel Level { get; set; } = GivtExceptionLevel.Error;

    protected GivtException(string message) : base(message)
    {
        AdditionalInformation = new Dictionary<string, object>();
    }

    public GivtException WithErrorTerm(string term)
    {
        AdditionalInformation.Add("errorTerm", term);
        return this;
    }
}