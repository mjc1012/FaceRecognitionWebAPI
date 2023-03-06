using FaceRecognitionWebAPI.Models;
using System.Security.Claims;

namespace FaceRecognitionWebAPI.Interfaces
{
    public interface IAuthenticationRepository
    {
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string expiredToken);
        public Task<bool> EmployeeRefreshTokenExists(string refreshToken);

        public Task<string> CreateRefreshToken();

        public Task<Person> saveTokens(Person person, string accessToken, string refreshToken);

        public Task<Person> saveTokens(Person person, string accessToken, string refreshToken, DateTime refreshTokenExpiryTime);

        public string CheckPasswordStrength(string password);

        public string CreateJwt(Person person);
    }
}
