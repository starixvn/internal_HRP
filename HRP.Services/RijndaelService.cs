namespace HRP.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using HRP.Models;

    public class RijndaelService : IRijndaelService
    {
        private const int FeedbackSize = 128;

        public (RijndaelKeyValuePair, RijndaelKeyValuePair) GenerateRijndaelKeyValuePair()
        {
            var rijndaelKey = new RijndaelKeyValuePair
            {
                Key = "RijndaelKey" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Value = Guid.NewGuid().ToString("N").Substring(0, 16)
            };

            var rijndaelIV = new RijndaelKeyValuePair
            {
                Key = "RijndaelIV" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                Value = Guid.NewGuid().ToString("N").Substring(0, 16)
            };

            return (rijndaelKey, rijndaelIV);
        }

        public string Decrypt(string cipherText, string rijndaelKey, string rijndaelIV)
        {
            string plainText = null;

            if (!string.IsNullOrEmpty(cipherText))
            {
                using (var rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.FeedbackSize = FeedbackSize;
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
            }

            return plainText;
        }

        public string Encrypt(string plainText, string rijndaelKey, string rijndaelIV)
        {
            string cipherText = null;

            if (!string.IsNullOrEmpty(plainText))
            {
                using (var rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.FeedbackSize = FeedbackSize;
                    rijAlg.Key = Encoding.UTF8.GetBytes(rijndaelKey);
                    rijAlg.IV = Encoding.UTF8.GetBytes(rijndaelIV);

                    var encryptor = rijAlg.CreateEncryptor();
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var srEncrypt = new StreamWriter(csEncrypt))
                            {
                                srEncrypt.Write(plainText);
                            }

                            cipherText = Convert.ToBase64String(msEncrypt.ToArray());
                        }
                    }
                }
            }

            return cipherText;
        }

        public void DecryptObject(object obj, string rijndaelKey, string rijndaelIV)
        {
            var securityObjects = obj.GetType().GetProperties()
                .Where(x => x.GetValue(obj) != null && (x.GetValue(obj).GetType().IsSubclassOf(typeof(SecurityData)) || x.GetValue(obj).GetType() == typeof(SecurityData)))
                .Select(x => x.GetValue(obj));

            foreach (var securityObject in securityObjects)
            {
                var type = securityObject.GetType();
                var encryptedValue = type.GetProperty(nameof(SecurityData.EncryptedValue));
                var plainValue = type.GetProperty(nameof(SecurityData.PlainValue));
                plainValue.SetValue(securityObject, Decrypt(encryptedValue.GetValue(securityObject)?.ToString(), rijndaelKey, rijndaelIV));
            }
        }

        public void EncryptObject(object obj, string rijndaelKey, string rijndaelIV)
        {
            var securityObjects = obj.GetType().GetProperties()
                .Where(x => x.GetValue(obj) != null && (x.GetValue(obj).GetType().IsSubclassOf(typeof(SecurityData)) || x.GetValue(obj).GetType() == typeof(SecurityData)))
                .Select(x => x.GetValue(obj));

            foreach (var securityObject in securityObjects)
            {
                var type = securityObject.GetType();
                var encryptedValue = type.GetProperty(nameof(SecurityData.EncryptedValue));
                var plainValue = type.GetProperty(nameof(SecurityData.PlainValue));
                encryptedValue.SetValue(securityObject, Encrypt(plainValue.GetValue(securityObject)?.ToString(), rijndaelKey, rijndaelIV));
            }
        }

        public void CleanPlainValue(object obj)
        {
            var securityObjects = obj.GetType().GetProperties()
                .Where(x => x.GetValue(obj) != null && (x.GetValue(obj).GetType().IsSubclassOf(typeof(SecurityData)) || x.GetValue(obj).GetType() == typeof(SecurityData)))
                .Select(x => x.GetValue(obj));

            foreach (var securityObject in securityObjects)
            {
                var type = securityObject.GetType();
                var plainValue = type.GetProperty(nameof(SecurityData.PlainValue));
                plainValue.SetValue(securityObject, null);
            }
        }
    }
}