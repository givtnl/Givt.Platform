using Givt.Platform.Common.Interfaces;

namespace Givt.Platform.Common.Entities;

public abstract class EntityBase : IEntity
{
    public Guid Id { get; set; }
}