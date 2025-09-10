
using App.Contracts.Object.Base.auth;
using Domain.Objects.Base;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace App.Object.Base.auth
{
    public class TokenApp : ITokenApp
    {




        public string GenerateToken(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.FullName)
    };

            // 🔒 هاردکد موقتی برای امضا
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key_at_least_16_chars")); // اینو بعداً می‌بری توی appsettings
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourapp",
                audience: "yourapp",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}