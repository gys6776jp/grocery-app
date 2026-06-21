using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GroceryApp.Application.Interfaces;
using GroceryApp.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GroceryApp.Infrastructure.Auth;

public class JwtService(IConfiguration config) : IJwtService
{
    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var expirationMinutes = int.TryParse(config["Jwt:ExpirationMinutes"], out var minutes) ? minutes : 20160; // 14 days
        var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);
        
        Console.WriteLine($"[JWT] Token generated for user {user.Username}, expires at: {expires} (UTC), expirationMinutes: {expirationMinutes}");
        
        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            },
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class PasswordService : IPasswordService
{
    public string Hash(string password) => BCrypt.Net.BCrypt.HashPassword(password);
    public bool Verify(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
}
