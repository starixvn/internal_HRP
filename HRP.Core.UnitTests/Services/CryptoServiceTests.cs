namespace HRP.Core.UnitTests.Services
{
    using System.Security.Cryptography.X509Certificates;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using Xunit;

    public class CryptoServiceTests
    {
        #region Encrypt

        [Theory]
        [InlineData(null)]
        [InlineData("  ")]
        public void Encrypt_PlainTextIsNullOrWhiteSpace_ReturnEmpty(string plainText)
        {
            // Arrange
            var cert = Substitute.For<X509Certificate2>();
            var configuration = FakeIOptionsSnapshot(true);
            var service = GetCryptoService(configuration, cert);

            // Act
            var cipherText = service.Encrypt(plainText);

            // Assert
            Assert.Equal(string.Empty, cipherText);
        }

        [Fact]
        public void Encrypt_DisableEncrypt_ReturnPlainText()
        {
            // Arrange
            var plainText = "123456";
            var cert = Substitute.For<X509Certificate2>();
            var configuration = FakeIOptionsSnapshot(false);
            var service = GetCryptoService(configuration, cert);

            // Act
            var cipherText = service.Encrypt(plainText);

            // Assert
            Assert.Equal(plainText, cipherText);
        }

        [Fact]
        public void Encrypt_EnableEncrypt_ExpectedEncryptSuccessfully()
        {
            // Arrange
            var plainText = "123456";
            var cert = GetX509Certificate2();
            var configuration = FakeIOptionsSnapshot(true);
            var service = GetCryptoService(configuration, cert);

            // Act
            var cipherText = service.Encrypt(plainText);

            // Assert
            Assert.NotEqual(plainText, cipherText);
        }

        #endregion Encrypt

        #region Decrypt

        [Theory]
        [InlineData(null)]
        [InlineData("  ")]
        public void Decrypt_CipherTextIsNullOrWhiteSpace_ReturnEmpty(string cipherText)
        {
            // Arrange
            var cert = Substitute.For<X509Certificate2>();
            var configuration = FakeIOptionsSnapshot(true);
            var service = GetCryptoService(configuration, cert);

            // Act
            var plainText = service.Decrypt(cipherText);

            // Assert
            Assert.Equal(string.Empty, plainText);
        }

        [Fact]
        public void Decrypt_DisableEncrypt_ReturnCipherText()
        {
            // Arrange
            var cipherText = "123456";
            var cert = Substitute.For<X509Certificate2>();
            var configuration = FakeIOptionsSnapshot(false);
            var service = GetCryptoService(configuration, cert);

            // Act
            var plainText = service.Decrypt(cipherText);

            // Assert
            Assert.Equal(cipherText, plainText);
        }

        [Fact]
        public void Decrypt_EnableEncrypt_ExpectedDecryptSuccessfully()
        {
            // Arrange
            var expected = "123456";
            var cert = GetX509Certificate2();
            var configuration = FakeIOptionsSnapshot(true);
            var service = GetCryptoService(configuration, cert);
            var cipherText = service.Encrypt(expected);

            // Act
            var actual = service.Decrypt(cipherText);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion Decrypt

        #region Helpers

        private static ICertificateService FakeCertificateService()
        {
            return Substitute.For<ICertificateService>();
        }

        private IOptionsSnapshot<AppSettingsOptions> FakeIOptionsSnapshot(bool enableEncrypt)
        {
            var appSettings = new AppSettingsOptions
            {
                EnableEncrypt = enableEncrypt
            };

            var config = Substitute.For<IOptionsSnapshot<AppSettingsOptions>>();
            config.Value.Returns(appSettings);

            return config;
        }

        private static X509Certificate2 GetX509Certificate2()
        {
            var resourceName = "HRP.Core.UnitTests.App_Data.SecretKey.pfx";
            byte[] embeddedCert;
            using (var stream = typeof(CryptoServiceTests).Assembly.GetManifestResourceStream(resourceName))
            {
                embeddedCert = new byte[stream.Length];
                stream.Read(embeddedCert, 0, (int)stream.Length);
            }

            return new X509Certificate2(embeddedCert, "123456");
        }

        private static CryptoService GetCryptoService(
            IOptionsSnapshot<AppSettingsOptions> config,
            X509Certificate2 cert)
        {
            ICertificateService certificateService = FakeCertificateService();
            var service = new CryptoService(certificateService, config, cert);

            return service;
        }

        #endregion Helpers
    }
}