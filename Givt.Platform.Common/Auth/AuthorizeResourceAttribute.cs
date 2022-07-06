using Givt.Platform.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Givt.Platform.Common.Auth;

public class AuthorizeResourceAttribute : Attribute, IAuthorizationFilter
{
    public Role Roles { get; set; }
    public GivtRole GivtRoles { get; set; }

    public AuthorizeResourceAttribute()
    {
    }

    private static Dictionary<string, GivtRole> _rolesMap = new()
    {
        // map role names (from exteral authentication provider(s)) to GivtRole flags
        { "Givt Admin", GivtRole.GivtAdmin },
        { "Givt Operator", GivtRole.GivtOperator },
    };

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        const string AUTH_NAMESPACE = "https://givtapp.net/auth";

        var claimsContext = context.HttpContext.User.Claims;

        // Check "super user" roles, where the user is authorised to work on all (Legal) Entities regardless his membership
        var claimedRoles = claimsContext.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).ToList();
        claimedRoles.ForEach(cr =>
        {
            if (_rolesMap.ContainsKey(cr) && ((_rolesMap[cr] | GivtRoles) != 0))
                return; // special "super user" role, allow access
        });

        var route = context.HttpContext.Request.Path.Value.ToLowerInvariant();

        //// Handle TemporaryAccess role
        //if (claimedRoles.Contains(GivtRoles.TemporaryAccess) && _roles.Contains(GivtRoles.TemporaryAccess))
        //{
        //    var userData = JsonSerializer.Deserialize<TokenUserData>(claimsContext.FirstOrDefault(x => x.Type == ClaimTypes.UserData)?.Value);
        //    if (!userData.Resources.Any(x => route.Contains(x.ToString().ToLowerInvariant())))
        //    {
        //        context.Result = new ForbidResult();
        //        return;
        //    }
        //}

        //// Assume authorized if there's a role that doesn't have resourcetype mapping
        //// This means this role has access without the need for specific resource type access
        //// e.g. siteAdmin, givtOperator
        //if (claimedRoles.Any(x => _roles.Contains(x) && !_resourceTypeRoleMapping.Keys.Contains(x)))
        //    return;

        // Authorisations on entities: the Guid in the route should be in the token "Auth" blob
        var authBlobs = claimsContext.Where(x => x.Type == AUTH_NAMESPACE).Select(x => x.Value).ToList();
        // a blob is a json string containing a Dictionary<Guid, Role> 
        foreach (var authBlob in authBlobs)
        {
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<Guid, Role>>(authBlob);
                if (dict.Keys.Any
                        (x =>
                            route.Contains(x.ToString(), StringComparison.OrdinalIgnoreCase) && // matchinig ID
                            ((Roles | dict[x]) != 0) // matching role
                        )
                    )
                    return; // user has a matching role on this ID
            }
            catch
            {
                // deserialisation error, do not authenticate
                // TODO: log
            }
        }

        // no match on required roles/ids
        context.Result = new ForbidResult();
    }
}
