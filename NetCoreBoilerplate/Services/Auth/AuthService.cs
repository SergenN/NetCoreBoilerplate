
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetCoreBoilerplate.Models.Settings;

namespace NetCoreBoilerplate.Services.Auth
{
    public class AuthService : IAuthService
    {
        
        private readonly JwtSettings _jwtSettings;

        public AuthService(IOptions<JwtSettings> jwtOptions)
        {
            _jwtSettings = jwtOptions.Value;
        }
        
        public string GenerateEncodedToken(string email, List<Claim> claims, bool rememberMe)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = rememberMe ? DateTime.Now.AddDays(Convert.ToDouble(_jwtSettings.Validity)) : DateTime.Now.AddDays(Convert.ToDouble(365));
            var token = new JwtSecurityToken(_jwtSettings.Issuer, _jwtSettings.Audience, claims, expires: expires, signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public List<Claim> GenerateClaimsIdentity(string email, string id)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, id)
            };
        }
    }
}