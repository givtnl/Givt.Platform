namespace Givt.Platform.Common.Exceptions;

public class ConcurrentUpdateException : GivtException
{
    public ConcurrentUpdateException() : base("Concurrent update happened, please try again")
    {
    }

    public override int ErrorCode => 409; // conflict
}
