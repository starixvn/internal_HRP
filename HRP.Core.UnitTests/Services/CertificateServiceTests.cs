namespace HRP.Core.UnitTests.Services
{
    using HRP.Core.Criteria;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Caching.Memory;
    using NSubstitute;
    using Xunit;

    public class CertificateServiceTests
    {
        private IHostingEnvironment hostingEnvironment;
        private IMemoryCache memoryCache;
        private IBaseService baseService;

        [Fact]
        public void GetCertificateFilePath_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var service = GetCertificateService();

            // Act
            var actual = service.GetCertificateFilePath();

            // Assert
            Assert.Equal("App_Data\\SecretKey.pfx", actual);
        }

        [Fact]
        public void GetPasswordAccessTheCertificate_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var service = GetCertificateService();

            // Act
            service.GetPasswordAccessTheCertificate();

            // Assert
            memoryCache.Received().TryGetValue(Arg.Any<object>(), out object outputValue);
        }

        [Fact]
        public void IsGeneratedCertificate_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var refResource = new ReferenceResourceModel { Value = "1" };
            var service = GetCertificateService();
            baseService.GetDataEntityAsync<ReferenceResourceModel>(Arg.Any<GetRefResourceByNamesCriteria>()).Returns(refResource);

            // Act
            var actual = service.IsGeneratedCertificate();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsExistedCertificate_WhenCalled_ReturnTrue()
        {
            // Arrange
            var service = GetCertificateService();

            // Act
            var actual = service.IsExistedCertificate();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsCorrectPasswordAccessTheCertificate_IsCorrectPassword_ReturnTrue()
        {
            // Arrange
            var service = GetCertificateService();

            // Act
            var actual = service.IsCorrectPasswordAccessTheCertificate("123456");

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsCorrectPasswordAccessTheCertificate_IsIncorrectPassword_ReturnFalse()
        {
            // Arrange
            var service = GetCertificateService();

            // Act
            var actual = service.IsCorrectPasswordAccessTheCertificate("123456aaAA@");

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void GenerateSelfSignedCertificate_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var certificateService = GetCertificateService();

            // Act
            var actual = certificateService.GenerateSelfSignedCertificate("123456");

            // Assert
            Assert.True(actual.IsSuccessful);
        }

        [Fact]
        public void CachePasswordAccessTheCertificate_WhenCalled_ExpectedCachingPassword()
        {
            // Arrange
            var certificateService = GetCertificateService();

            // Act
            certificateService.CachePasswordAccessTheCertificate("123456");

            // Assert
            memoryCache.Received().Set("PasswordAccessTheCertificate", "123456", Arg.Any<MemoryCacheEntryOptions>());
        }

        #region Helpers

        private static IHostingEnvironment FakeHostingEnvironment()
        {
            return Substitute.For<IHostingEnvironment>();
        }

        private static IMemoryCache FakeMemoryCache()
        {
            return Substitute.For<IMemoryCache>();
        }

        private static IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private CertificateService GetCertificateService()
        {
            hostingEnvironment = FakeHostingEnvironment();
            memoryCache = FakeMemoryCache();
            baseService = FakeBaseService();
            var service = new CertificateService(hostingEnvironment, memoryCache, baseService);

            return service;
        }

        #endregion Helpers
    }
}