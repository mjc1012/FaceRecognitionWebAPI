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

namespace FaceRecognitionWebAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly DataContext _context;
        public AuthenticationService(DataContext context)
        {
            _context = context;
        }


        public string CheckPasswordStrength(string password)
        {
            try
            {
                StringBuilder sb = new();
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
            catch(Exception ) 
            {
                throw;
            }
        }

        public string CreateJwt(Person person)
        {
            try
            {
                JwtSecurityTokenHandler jwtTokenHandler = new();
                byte[] key = Encoding.ASCII.GetBytes("veryveryverysecret.....");
                ClaimsIdentity identity = new(new Claim[]
                {
                new Claim(ClaimTypes.Name, person.PairId)
                });

                SigningCredentials credentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

                SecurityTokenDescriptor tokenDescriptor = new()
                {
                    Subject = identity,
                    Expires = DateTime.Now.AddMinutes(5),
                    SigningCredentials = credentials,
                };
                SecurityToken token = jwtTokenHandler.CreateToken(tokenDescriptor);
                return jwtTokenHandler.WriteToken(token);
            }
            catch(Exception)
            {
                throw;
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string expiredToken)
        {
            try
            {
                byte[] key = Encoding.ASCII.GetBytes("veryveryverysecret.....");
                TokenValidationParameters tokenValidationParameters = new ()
                {
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = false
                };
                JwtSecurityTokenHandler tokenHandler = new();
                ClaimsPrincipal principal = tokenHandler.ValidateToken(expiredToken, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new SecurityTokenException("This is an invalid token");
                }
                else
                {
                    return principal;
                }
            }
            catch (Exception)
            {
                throw;
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
            try
            {
                byte[] tokenBytes = RandomNumberGenerator.GetBytes(64);
                string refreshToken = Convert.ToBase64String(tokenBytes);

                bool tokenInUser = await EmployeeRefreshTokenExists(refreshToken);

                if (tokenInUser)
                {
                    return await CreateRefreshToken();
                }
                return refreshToken;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<Person> saveTokens(Person person, string accessToken, string refreshToken)
        {
            try
            {
                person.AccessToken = accessToken;
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
                person.AccessToken = accessToken;
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
