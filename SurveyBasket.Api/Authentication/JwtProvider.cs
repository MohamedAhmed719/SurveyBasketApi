using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SurveyBasket.Api.Authentication;

public class JwtProvider(IOptions<JwtOptions> jwtOptions) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public (string Token, int ExpiresIn) GenerateJwtToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        Claim[] claims = [
            new(JwtRegisteredClaimNames.Sub,user.Id),
            new(JwtRegisteredClaimNames.GivenName,user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName,user.LastName),
            new(JwtRegisteredClaimNames.Email,user.Email),
            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new(nameof(roles),JsonSerializer.Serialize(roles),JsonClaimValueTypes.JsonArray),
            new(nameof(permissions),JsonSerializer.Serialize(permissions),JsonClaimValueTypes.JsonArray)

            ];

        var expiresIn = _jwtOptions.ExpiryMinutes;

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
             issuer: _jwtOptions.Issuer,
             audience: _jwtOptions.Audience,
             signingCredentials: signingCredentials,
             claims: claims,
             expires: DateTime.UtcNow.AddMinutes(expiresIn)
            );

        return (Token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: expiresIn * 60);
    }

    public string? ValidateToken(string Token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

        try
        {
            tokenHandler.ValidateToken(Token, new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = symmetricSecurityKey,
                ClockSkew = TimeSpan.Zero
            }
            , out SecurityToken validatedToken
            );

            var jwtToken = (JwtSecurityToken)validatedToken;

            return jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        }
        catch 
        {
            return null;
        }

        
    }
}
