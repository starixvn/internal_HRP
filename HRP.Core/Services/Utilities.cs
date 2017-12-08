namespace HRP.Core.Services
{
    using System;
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
            var random = Guid.NewGuid().ToString();
            random = random.Replace("-", string.Empty).Substring(0, 12);
            return random;
        }
    }
}