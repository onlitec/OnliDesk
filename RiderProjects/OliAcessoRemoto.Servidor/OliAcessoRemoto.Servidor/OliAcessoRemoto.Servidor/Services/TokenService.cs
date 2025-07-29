using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OliAcessoRemoto.Servidor.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string clientId, string clientName)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "OnliDekRemoteAccessSecretKey2024!");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("clientId", clientId),
                new Claim("clientName", clientName),
                new Claim(ClaimTypes.NameIdentifier, clientId)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"] ?? "OnliDekServer",
            Audience = _configuration["Jwt:Audience"] ?? "OnliDekClients"
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
