using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Serilog.Sinks.Http.Logger;
using Givt.Platform.JWT.Options;

namespace Givt.Platform.JWT.Handlers;

public class JwtTokenHandler
{
    private const string S_TRANSACTIONREFERENCE = "txref";

    private readonly ILog _log;
    private readonly JwtOptions _options;

    public JwtTokenHandler(ILog log, JwtOptions options)
    {
        _log = log;
        _options = options;
    }

    public string GetBearerToken(string transactionReference)
    {
        _log.Debug($"Creating JwtToken for '{transactionReference}'");
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString("N")), // a dummy user name
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(S_TRANSACTIONREFERENCE, transactionReference)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.IssuerSigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;
        var expires = now.AddHours(Convert.ToDouble(_options.ExpireHours));

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience, // ???
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token); // jwtEncodedString
    }

    public string GetTransactionReference(ClaimsPrincipal claimsPrincipal)
    {
        var result = claimsPrincipal
            ?.Claims
            ?.FirstOrDefault(c => c.Type.Equals(S_TRANSACTIONREFERENCE, StringComparison.OrdinalIgnoreCase))
            ?.Value;
        if (String.IsNullOrEmpty(result))
            throw new UnauthorizedAccessException("Bearer does not have a transaction reference");
        _log.Debug($"Bearer has transaction reference '{result}'");
        return result;
    }

}