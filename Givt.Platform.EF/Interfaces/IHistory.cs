using Givt.Platform.EF.Enums;

namespace Givt.Platform.EF.Interfaces;

public interface IHistory
{
    public Guid Id { get; set; }
    public DateTime Modified { get; set; }
    public EntityLogReason Reason { get; set; }
}