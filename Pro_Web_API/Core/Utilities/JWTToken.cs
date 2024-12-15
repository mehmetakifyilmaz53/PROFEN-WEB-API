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
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("b1f6c3f8e2d94b5eaef5ac39484c9476")); 

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

       
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.user_Name),
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Role, user.role.ToString()) 
    };

       
            var token = new JwtSecurityToken(
                issuer: "localhost",               
                audience: "localhost",             
                claims: claims,
                expires: DateTime.Now.AddHours(1), 
                signingCredentials: creds
            );

      
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
