using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Givt.Platform.Common.MiddleWare;

public class MultipleSchemaAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public MultipleSchemaAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var principal = new ClaimsPrincipal();

        var resultLocal = await context.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        if (resultLocal?.Principal != null)
        {
            principal.AddIdentities(resultLocal.Principal.Identities);
        }

        var resultAuth0 = await context.AuthenticateAsync("Auth0");
        if (resultAuth0?.Principal != null)
        {
            principal.AddIdentities(resultAuth0.Principal.Identities);
        }

        context.User = principal;

        await _next(context);
    }
}
