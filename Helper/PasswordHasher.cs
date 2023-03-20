using System.Security.Cryptography;

namespace FaceRecognitionWebAPI.Helper
{
    public class PasswordHasher
    {
        private static readonly RNGCryptoServiceProvider rng = new();
        private static readonly int saltSize = 16;
        private static readonly int hashSize = 16;
        private static readonly int iterations = 10000;

        public static string HashPassword(string password)
        {
            try
            {
                byte[] salt;
                rng.GetBytes(salt = new byte[saltSize]);
                Rfc2898DeriveBytes key = new(password, salt, iterations);
                byte[] hash = key.GetBytes(hashSize);
                byte[] hashBytes = new byte[saltSize + hashSize];
                Array.Copy(salt, 0, hashBytes, 0, saltSize);
                Array.Copy(hash, 0, hashBytes, saltSize, hashSize);
                string base64Hash = Convert.ToBase64String(hashBytes);
                return base64Hash;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public static bool VerifyPassword(string password, string base64Hash)
        {
            try
            {
                byte[] hashBytes = Convert.FromBase64String(base64Hash);
                byte[] salt = new byte[saltSize];
                Array.Copy(hashBytes, 0, salt, 0, saltSize);
                Rfc2898DeriveBytes key = new(password, salt, iterations);
                byte[] hash = key.GetBytes(hashSize);

                for (int i = 0; i < hashSize; i++)
                {
                    if (hashBytes[i + saltSize] != hash[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}
