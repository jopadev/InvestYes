using InvestYes.Application.Interfaces;
using InvestYes.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InvestYes.Application.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string Generate(User user)
    {
        var sJwt = _configuration.GetSection("Jwt");

        var sKey = Encoding.UTF8.GetBytes(sJwt["SecretKey"]!);

        var oClaimList = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),

            new Claim(JwtRegisteredClaimNames.Email, user.Email),

            new Claim(ClaimTypes.Name, user.Name),

            new Claim(ClaimTypes.Role, user.Role)
        };

        var oSigningCredentials = new SigningCredentials(new SymmetricSecurityKey(sKey),SecurityAlgorithms.HmacSha256);

        var oJwtSecurityToken = new JwtSecurityToken(
            issuer: sJwt["Issuer"],
            audience: sJwt["Audience"],
            claims: oClaimList,
            expires: DateTime.UtcNow.AddMinutes(int.Parse(sJwt["ExpirationMinutes"]!)),
            signingCredentials: oSigningCredentials);

        return new JwtSecurityTokenHandler().WriteToken(oJwtSecurityToken);
    }
}


