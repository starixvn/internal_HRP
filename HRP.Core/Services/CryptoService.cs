namespace HRP.Core.Services
{
    using System;
    using System.Security.Cryptography;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using HRP.Core.CustomException;
    using HRP.Core.Model;
    using HRP.Resources;
    using Microsoft.Extensions.Options;

    public class CryptoService : ICryptoService, IDisposable
    {
        private const bool OptimalAsymmetricEncryptionPadding = false;

        private readonly X509Certificate2 cert2;
        private readonly ICertificateService certificateService;
        private readonly IOptionsSnapshot<AppSettingsOptions> configuration;

        public CryptoService(
            ICertificateService certificateService,
            IOptionsSnapshot<AppSettingsOptions> configuration,
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
            var plainText = string.Empty;

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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            cert2.Dispose();
        }

        private X509Certificate2 InitCertificate()
        {
            if (!certificateService.IsExistedCertificate())
            {
                throw new CertificateNotFoundException(Message.CertificateNotFound);
            }

            try
            {
                string filePath = certificateService.GetCertificateFilePath();
                string password = certificateService.GetPasswordAccessTheCertificate();
                var flags = X509KeyStorageFlags.MachineKeySet;

                return new X509Certificate2(filePath, password, flags);
            }
            catch (CryptographicException ex)
            {
                throw new CertificatePasswordException(
                    ex.Message + Environment.NewLine + ex.Source
                    + Environment.NewLine + ex.TargetSite
                    + Environment.NewLine + ex.StackTrace);
            }
        }
    }
}