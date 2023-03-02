﻿using Dapper;
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
using Microsoft.AspNetCore.Identity;

namespace WebAPI_HD.Repository
{
    public interface IJwtAuthenticationManager
    {
        public JwtSecurityToken GenerateAccessToken(List<Claim> authClaims);
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
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JWTSettings:SecretKey"]!));
            _ = int.TryParse(Configuration["JWTSettings:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var tokenDescriptor = new JwtSecurityToken
            (
                //issuer: Configuration["JWT:ValidIssuer"],
                //audience: Configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddSeconds(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            //var cookieOptions = new CookieOptions
            //{
            //    HttpOnly = true,
            //    SameSite = SameSiteMode.Strict,
            //    Secure = true,
            //    Expires = DateTime.Now.AddDays(7),
            //    MaxAge = TimeSpan.FromDays(7)
            //};
            return tokenDescriptor;
        }
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWTSettings:SecretKey"]!)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;

        }
        //public string GenerateToken(IdentityUser user)
        //{
        //    // generate token that is valid for 7 days
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var key = Encoding.ASCII.GetBytes(Configuration["JWTSettings:SecretKey"]!);
        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Subject = new ClaimsIdentity(new[] { new Claim("UserName", user.UserName!)}),
        //        Expires = DateTime.UtcNow.AddHours(3),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //    };
        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    return tokenHandler.WriteToken(token);
        //}
    }
}
