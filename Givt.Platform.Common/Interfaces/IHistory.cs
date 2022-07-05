using Givt.Platform.Common.Enums;

namespace Givt.Platform.Common.Interfaces;

public interface IHistory
{
    public Guid Id { get; set; }
    public DateTime Modified { get; set; }
    public EntityLogReason Reason { get; set; }
}