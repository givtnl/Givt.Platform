namespace Givt.Platform.Common.Entities;

public abstract class EntityLockAudit<Ttoken> : EntityAudit
{
    public Ttoken ConcurrencyToken { get; set; }
}
