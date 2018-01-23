namespace HRP.Core.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using Microsoft.Extensions.Options;
    using Model;

    public class CryptoService : ICryptoService, IDisposable
    {
        private const bool OptimalAsymmetricEncryptionPadding = false;
        private X509Certificate2 cert2;
        private readonly IOptionsSnapshot<AppSettingsOptions> configuration;

        public CryptoService(IOptionsSnapshot<AppSettingsOptions> configuration)
        {
            this.configuration = configuration;
            InitCertificate();
        }

        public CryptoService(X509Certificate2 cert)
        {
            cert2 = cert;
        }

        public string Encrypt(string plainText)
        {
            string cipherText = string.Empty;

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                if (configuration.Value.EnableEncrypt)
                {
                    byte[] bytesToEncrypt = ASCIIEncoding.ASCII.GetBytes(plainText);

                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert2.PublicKey.Key;
                    byte[] encryptedBytes = rsa.Encrypt(bytesToEncrypt, OptimalAsymmetricEncryptionPadding);
                    cipherText = Convert.ToBase64String(encryptedBytes);
                }
                else
                {
                    cipherText = plainText;
                }
            }

            return cipherText;
        }

        public string Decrypt(string cipherText)
        {
            string plainText = string.Empty;

            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                if (configuration.Value.EnableEncrypt)
                {
                    RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)cert2.PrivateKey;
                    byte[] bytesToDecrypt = Convert.FromBase64String(cipherText);
                    byte[] plainbytes = rsa.Decrypt(bytesToDecrypt, OptimalAsymmetricEncryptionPadding);
                    ASCIIEncoding enc = new ASCIIEncoding();
                    plainText = enc.GetString(plainbytes);
                }
                else
                {
                    plainText = cipherText;
                }
            }

            return plainText;
        }

        public void InitCertificate()
        {
            if (cert2 == null)
            {
                X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet;
                cert2 = new X509Certificate2(configuration.Value.CertificateFilePath, configuration.Value.PasswordToSignTheCertificateFile, flags);
            }
        }

        public void Dispose()
        {
            cert2.Dispose();
        }
    }
}