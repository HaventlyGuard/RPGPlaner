using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace UserService.Models;

public class JwtProvider (IOptions <JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = options.Value;
    public string CreateToken(User user)
    {
        Claim[] claims = 
        [
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("userId", user.Id.ToString())
        ];
        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret)), SecurityAlgorithms.HmacSha256Signature);
        var token = new JwtSecurityToken(
            claims: claims,
            signingCredentials: signingCredentials,
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            expires: DateTime.UtcNow.AddHours(_jwtOptions.ExpiresHours));
            
            var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return tokenValue;
        
    }
}

public class JwtOptions : IJwtOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Secret { get; set; }
    public int ExpiresHours { get; set; } = 20;
}

public interface IJwtOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Secret { get; set; }
    public int ExpiresHours { get; set; } 
}

public interface IJwtProvider
{
    public string CreateToken(User user);
}