using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace KiwdyAPI.Services
{
    public class HashService
    {
        public static string HashClave(string salt, string clave)
        {
            return Convert.ToBase64String(
                KeyDerivation.Pbkdf2(
                    password: clave,
                    salt: System.Text.Encoding.ASCII.GetBytes(salt),
                    prf: KeyDerivationPrf.HMACSHA1,
                    iterationCount: 1000,
                    numBytesRequested: 256 / 8
                )
            );
        }

        public static bool VerificarClave(string salt, string claveRequest, string claveHashed)
        {
            return (HashClave(salt, claveRequest) == claveHashed);
        }
    }
}
