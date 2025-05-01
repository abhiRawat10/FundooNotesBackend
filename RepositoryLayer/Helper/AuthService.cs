
using System.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RepositoryLayer.Interfaces;
using ModelLayer.Entity;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;

namespace RepositoryLayer.Helper
{
    public class AuthService 
    {
        private readonly IConfiguration _configuration;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //public string DecodeJwtToken(string token)
        //{
        //    var handler = new JwtSecurityTokenHandler();
        //    // Check if the token is valid
        //    if(token==null) throw new ArgumentException("Invalid JWT token");
        //    //if (!handler.CanReadToken(token))
        //    //{
        //    //    throw new ArgumentException("Invalid JWT token");
        //    //}
        //    var jwtToken = handler.ReadJwtToken(token);

        //    foreach (var claim in jwtToken.Claims)
        //    {   if(claim.Type=="Email")return claim.Value;
        //        Console.WriteLine($"{claim.Type}: {claim.Value}");
        //    }
        //    // if you havent used custom claim types than this is the method => var name = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        //    var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "Email")?.Value;
        //    Console.WriteLine("kjsdfn   "+email);
        //    // Return the full token payload and specific claims
        //    return email;

        //}


        public async Task<String?> GenerateJwtToken(UserEntity user)
        {
            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));


            //•	Signing credentials are used to specify the algorithm and key that will be used to sign the JWT token. This ensures that the token is signed using a secure algorithm and the specified key.
            var credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("UserId",user.Id.ToString()),
                new Claim("Email",user.Email)
            };

            var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateOtp()
        {
            var random = new Random();
            string otp = "";
            for (int i = 0; i < 6; i++)
            {
                otp += random.Next(0, 10); // generates a digit from 0 to 9
            }
            return otp;
        }

    }
}



//namespace BusinessLayer.Helpers
//{
//    public class TokenServiceBL : ITokenServiceBL
//    {
//        private readonly IConfiguration _configuration;

//        public TokenServiceBL(IConfiguration configuration)
//        {
//            _configuration = configuration;
//        }

//        public async Task<string> GenerateJwtTokenAsync(UserEntity user)
//        {
//            return await Task.Run(() =>
//            {
//                var jwtSettings = _configuration.GetSection("JwtSettings");

//                var claims = new[]
//                {
//                    new Claim("Id", user.Id.ToString()),
//                    new Claim("Email", user.Email),
//                    new Claim("FirstName", user.FirstName),
//                    new Claim("LastName", user.LastName),
//                    new Claim(ClaimTypes.Role, user.Role)
//                };

//                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
//                var token = new JwtSecurityToken(
//                    issuer: jwtSettings["Issuer"],
//                    audience: jwtSettings["Audience"],
//                    claims: claims,
//                    expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["ExpirationMinutes"])),
//                    signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256)
//                );

//                return new JwtSecurityTokenHandler().WriteToken(token);
//            });
//        }
//    }
//}