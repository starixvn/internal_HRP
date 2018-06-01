namespace HRP.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    public class RijndaelService : IRijndaelService
    {
        public Tuple<string, string> GenerateKey()
        {
            string ivStr = Guid.NewGuid().ToString("N").Substring(0, 16);
            string keyStr = Guid.NewGuid().ToString("N").Substring(0, 16);

            return new Tuple<string, string>(ivStr, keyStr);
        }

        public string Decrypt(string cipherText, string rijndaelKey, string rijndaelIV)
        {
            string plainText = string.Empty;

            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;
                rijAlg.Key = Encoding.UTF8.GetBytes(rijndaelKey);
                rijAlg.IV = Encoding.UTF8.GetBytes(rijndaelIV);

                var decryptor = rijAlg.CreateDecryptor();
                using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plainText = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plainText;
        }
    }
}