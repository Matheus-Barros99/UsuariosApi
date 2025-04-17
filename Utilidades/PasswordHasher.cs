using System.Security.Cryptography;
using System.Text;

namespace UsuariosApi.Utilidades
{
    public static class PasswordHasher
    {
        public static string SaltGenerator()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashSenha(string senha, string salt)
        {
            var combined = Encoding.UTF8.GetBytes(senha + salt);
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(combined);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerificaSenha(string senha, string salt, string senhaHashed)
        {
            var hashToCheck = HashSenha(senha, salt);

            return hashToCheck == senhaHashed;
        }
    }
}
