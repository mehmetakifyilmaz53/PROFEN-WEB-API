using Microsoft.IdentityModel.Tokens;
using Pro_Web_API.Core.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pro_Web_API.Core.Utilities
{
    public class JWTToken
    {
        public static string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("b1f6c3f8e2d94b5eaef5ac39484c9476")); // Rastgele GUID

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Kullanıcı bilgileri ve rollerini içeren claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.email),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.role.ToString()) // Kullanıcının rolü
    };

            // Token yapılandırması
            var token = new JwtSecurityToken(
                issuer: "localhost",               // Issuer
                audience: "localhost",             // Audience
                claims: claims,
                expires: DateTime.Now.AddHours(1),   // Token geçerlilik süresi
                signingCredentials: creds
            );

            // Token string'e dönüştürülür
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
