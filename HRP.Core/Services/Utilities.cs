namespace HRP.Core.Services
{
    using BCrypt.Net;

    public static class Utilities
    {
        public static string GenerateRadomString()
        {
            string salt = BCrypt.GenerateSalt(30);
            string random = salt.Replace("/", string.Empty).Replace(".", string.Empty);
            return random;
        }

        public static string GenerateSecretKey()
        {
            return BCrypt.GenerateSalt(12);
        }
    }
}