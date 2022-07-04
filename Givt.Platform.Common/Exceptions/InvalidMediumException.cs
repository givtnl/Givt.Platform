namespace Givt.Platform.Common.Exceptions;

public class InvalidMediumException : GivtException
{
    public InvalidMediumException(string property, string value) : base($"Medium {property} has invalid value: {value}") { }

    public override int ErrorCode => 417;
}