using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using QubeFin.Core.Settings;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QubeFin.Core.Security;

public interface ITokenGenerator
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
}

public class TokenGenerator(IOptions<JwtSettings> jwtSettings) : ITokenGenerator
{
    public readonly JwtSettings _jwtSettings = jwtSettings.Value;

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SigningSecret));
        var credsSigning = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var handler = new JsonWebTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtSettings.Audience,
            Issuer = _jwtSettings.Issuer,
            Expires = DateTime.UtcNow.AddDays(7),
            Claims = claims.GroupBy(i => i.Type).ToDictionary(i => i.Key, i => (object)(i.Count() == 1 ? i.First().Value : i.Select(i => i.Value).ToArray())),
            SigningCredentials = credsSigning,
        };

        return handler.CreateToken(tokenDescriptor);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}

