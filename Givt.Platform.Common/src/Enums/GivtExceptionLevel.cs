namespace Givt.Platform.Common.Enums;

public enum GivtExceptionLevel
{

    /// <summary>
    /// The lifeblood of operational intelligence - things
    /// happen.
    /// </summary>
    Information,
    /// <summary>
    /// Service is degraded or endangered.
    /// </summary>
    Warning,
    /// <summary>
    /// Functionality is unavailable, invariants are broken
    /// or data is lost.
    /// </summary>
    Error
}