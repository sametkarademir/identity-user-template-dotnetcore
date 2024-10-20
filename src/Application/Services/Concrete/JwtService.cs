using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Services.Interface;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Concrete;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly UserManager<AppUser> _userManager;
    private readonly SymmetricSecurityKey _jwtKey;
    
    public JwtService(IConfiguration config, UserManager<AppUser> userManager)
    {
        _config = config;
        _userManager = userManager;
        _jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]!));
    }
    
    public async Task<string> CreateJwtAsync(AppUser user)
    {
        var userClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? String.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? String.Empty)
        };

        var roles = await _userManager.GetRolesAsync(user);
        userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var creadentials = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha512Signature);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(userClaims),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_config["JWT:ExpiresInMinutes"] ?? "60")),
            SigningCredentials = creadentials,
            Issuer = _config["JWT:Issuer"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(jwt);
    }

    public RefreshToken CreateRefreshToken(AppUser user)
    {
        var token = new byte[32];
        using var randomNumberGenerator = RandomNumberGenerator.Create();
        randomNumberGenerator.GetBytes(token);

        var tokenString = Convert.ToBase64String(token)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");
        
        var alphanumericToken = new StringBuilder();
        var random = new Random();

        foreach (var c in tokenString)
        {
            if (char.IsLetterOrDigit(c))
            {
                alphanumericToken.Append(c);
            }
            else
            {
                alphanumericToken.Append((char)random.Next(48, 58));
            }
        }

        var refreshToken = new RefreshToken()
        {
            Token = alphanumericToken.ToString(),
            AppUser = user,
            DateExpiresUtc = DateTime.UtcNow.AddDays(int.Parse(_config["JWT:RefreshTokenExpiresInDays"] ?? "1"))
        };

        return refreshToken;
    }
}