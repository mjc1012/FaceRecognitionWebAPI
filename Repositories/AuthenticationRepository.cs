using FaceRecognitionWebAPI.Data;
using FaceRecognitionWebAPI.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using FaceRecognitionWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FaceRecognitionWebAPI.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly DataContext _context;
        public AuthenticationRepository(DataContext context)
        {
            _context = context;
        }


        public string CheckPasswordStrength(string password)
        {
            StringBuilder sb = new StringBuilder();
            if (password.Length < 8)
            {
                sb.Append("Minimum password length should be 8" + Environment.NewLine);
            }
            if (!(Regex.IsMatch(password, "[a-z]") && Regex.IsMatch(password, "[A-Z]") && Regex.IsMatch(password, "[0-9]")))
            {
                sb.Append("Password should contain atleast one Uppercase Letter, one Lowercase Letter and one Number" + Environment.NewLine);
            }
            if (!Regex.IsMatch(password, "[~,',!,@,#,$,%,^,&,*,(,),-,_,+,=,{,},\\[,\\],|,/,\\,:,;,\",`,<,>,,,.,?]"))
            {
                sb.Append("Password should contain contain atleast one special character" + Environment.NewLine);
            }
            return sb.ToString();
        }

        public string CreateJwt(Person person)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("veryveryverysecret.....");
            var identity = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, person.ValidIdNumber)
            });

            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = identity,
                Expires = DateTime.Now.AddSeconds(10),
                SigningCredentials = credentials,
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            return jwtTokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string expiredToken)
        {
            var key = Encoding.ASCII.GetBytes("veryveryverysecret.....");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(expiredToken, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("This is an invalid token");
            }
            else
            {
                return principal;
            }
        }

        public async Task<bool> EmployeeRefreshTokenExists(string refreshToken)
        {
            try
            {
                return await _context.Persons.AnyAsync(p => p.RefreshToken == refreshToken.Trim());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> CreateRefreshToken()
        {
            var tokenBytes = RandomNumberGenerator.GetBytes(64);
            var refreshToken = Convert.ToBase64String(tokenBytes);

            var tokenInUser = await EmployeeRefreshTokenExists(refreshToken);

            if (tokenInUser)
            {
                return await CreateRefreshToken();
            }
            return refreshToken;
        }

        public async Task<Person> saveTokens(Person person, string accessToken, string refreshToken)
        {
            try
            {
                person.Token = accessToken;
                person.RefreshToken = refreshToken;
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Person> saveTokens(Person person, string accessToken, string refreshToken, DateTime refreshTokenExpiryTime)
        {
            try
            {
                person.Token = accessToken;
                person.RefreshToken = refreshToken;
                person.RefreshTokenExpiryTime = refreshTokenExpiryTime;
                await _context.SaveChangesAsync();
                return person;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
