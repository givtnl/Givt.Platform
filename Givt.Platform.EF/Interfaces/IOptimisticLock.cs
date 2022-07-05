namespace Givt.Platform.EF.Interfaces;

public interface IOptimisticLock<Ttoken>: IAuditBasic
{
    Ttoken ConcurrencyToken { get; set; }
}
