namespace Givt.Platform.EF.Entities;

public abstract class EntityAudit : EntityBase
{
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}