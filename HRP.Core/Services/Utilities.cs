namespace HRP.Core.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using BCrypt.Net;
    using HRP.Core.Constants;

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

        public static string DecryptStringAES(string cipherText)
        {
            var keybytes = Encoding.UTF8.GetBytes(Constants.RijndaelKey);
            var iv = Encoding.UTF8.GetBytes(Constants.RijndaelIV);

            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);
            return string.Format(decriptedFromJavascript);
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            string decrypted = string.Empty;

            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = key;
                rijAlg.IV = iv;

                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                decrypted = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
                catch
                {
                    decrypted = string.Empty;
                }
            }

            return decrypted;
        }
    }
}