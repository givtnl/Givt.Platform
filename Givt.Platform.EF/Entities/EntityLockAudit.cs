namespace Givt.Platform.EF.Entities;

public abstract class EntityLockAudit<Ttoken> : EntityAudit
{
    public Ttoken ConcurrencyToken { get; set; }
}
