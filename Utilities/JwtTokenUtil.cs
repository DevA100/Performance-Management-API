using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PerformanceSurvey.Configuration;
using PerformanceSurvey.iServices;
using PerformanceSurvey.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PerformanceSurvey.Utilities
{
    public class JwtTokenUtil
    {


        private readonly JwtSettings _jwtSettings;
        public JwtTokenUtil(IOptions<JwtSettings> jwtsettings)
        {
            _jwtSettings = jwtsettings.Value;
        }

        public string GenerateToken(string UserEmail, string Role)
        {
            var claims = new[] { 
                
                new Claim(ClaimTypes.NameIdentifier, UserEmail),
                 new Claim(ClaimTypes.Role, Role),
                  

            };

            var secret = _jwtSettings.Key;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

            var creds = new SigningCredentials(key,
             SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,

                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(60),

                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

      

    }

}

