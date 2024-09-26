namespace HRP.Core.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using HRP.Core.Model;
    using HRP.Resources;

    public class CryptoService : ICryptoService
    {
        private readonly X509Certificate2 cert2;
        private readonly ICertificateService certificateService;
        private readonly AppSettingsOptions configuration;

        public CryptoService(
            ICertificateService certificateService,
            AppSettingsOptions configuration,
            X509Certificate2 cert2 = null)
        {
            this.certificateService = certificateService;
            this.configuration = configuration;
            this.cert2 = cert2 ?? InitCertificate();
        }

        public string Encrypt(string plainText)
        {
            var cipherText = string.Empty;

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                if (configuration.EnableEncrypt)
                {
                    byte[] bytesToEncrypt = ASCIIEncoding.ASCII.GetBytes(plainText);

                    var rsa = cert2.GetRSAPublicKey();
                    byte[] encryptedBytes = rsa.Encrypt(bytesToEncrypt, RSAEncryptionPadding.Pkcs1);
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
            var plainText = string.Empty;

            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                if (configuration.EnableEncrypt)
                {
                    byte[] bytesToDecrypt = Convert.FromBase64String(cipherText);
                    var rsa = cert2.GetRSAPrivateKey();
                    byte[] plainbytes = rsa.Decrypt(bytesToDecrypt, RSAEncryptionPadding.Pkcs1);
                    var enc = new ASCIIEncoding();
                    plainText = enc.GetString(plainbytes);
                }
                else
                {
                    plainText = cipherText;
                }
            }

            return plainText;
        }

        private X509Certificate2 InitCertificate()
        {
            if (!certificateService.IsExistedCertificate())
            {
                throw new FileNotFoundException(Message.CertificateNotFound);
            }

            string filePath = certificateService.GetCertificateFilePath();
            string password = certificateService.GetPasswordAccessTheCertificate();
            const X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet;

            return new X509Certificate2(filePath, password, flags);
        }
    }
}