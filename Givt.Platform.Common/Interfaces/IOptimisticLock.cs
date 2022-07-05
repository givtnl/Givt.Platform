namespace Givt.Platform.Common.Interfaces;

public interface IOptimisticLock<Ttoken>: IAuditBasic
{
    Ttoken ConcurrencyToken { get; set; }
}
