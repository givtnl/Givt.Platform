using Givt.Platform.EF.Interfaces;

namespace Givt.Platform.EF.Entities;

public abstract class EntityBase : IEntity
{
    public Guid Id { get; set; }
}