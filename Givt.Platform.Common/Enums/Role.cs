namespace Givt.Platform.Common.Enums;

public enum Role
{
    // numbers are used to provide permission checks: if ((user.Role | requiredRole(s)) != 0) --> allow access
    CampaignManager = 0x0001,
    Treasurer = 0x0002,

    Donor = 0x0100,
}