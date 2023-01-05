using Dapper;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPI_HD.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;

namespace WebAPI_HD.Repository
{
    public interface IJwtAuthenticationManager
    {
        public JwtSecurityToken GenerateAccessToken(List<Claim> authClaims);
        public int? ValidateToken(string token);
    }
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly IConfiguration Configuration;

        private readonly JWTSettings _jwtsettings;
        public JwtAuthenticationManager(IOptions<JWTSettings> jwtsettings, IConfiguration configuration)

        {
            _jwtsettings = jwtsettings.Value;
            Configuration = configuration;
        }

        public JwtSecurityToken GenerateAccessToken(List<Claim> authClaims)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["JWTSettings:SecretKey"]);
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JWTSettings:SecretKey"]));
            var tokenDescriptor = new JwtSecurityToken
            (
                /*     Subject = new ClaimsIdentity(authClaims),
                     Expires = DateTime.UtcNow.AddDays(7),
                     SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)*/
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
           /* var token = tokenHandler.CreateToken(tokenDescriptor);*/
            return tokenDescriptor;
        }
        public int? ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["JWTSettings:SecretKey"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

                // return user id from JWT token if validation successful
                return userId;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }

    }
}
