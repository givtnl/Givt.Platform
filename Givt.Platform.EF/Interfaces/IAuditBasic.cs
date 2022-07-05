namespace Givt.Platform.EF.Interfaces;

public interface IAuditBasic
{
    DateTime Created { get; set; }
    DateTime Modified { get; set; }
}