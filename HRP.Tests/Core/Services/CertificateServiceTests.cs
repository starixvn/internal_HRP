namespace HRP.Tests.Core.Services
{
    using HRP.Core.Model;
    using HRP.Core.Services;
    using Microsoft.Extensions.Options;
    using Xunit;

    public class CertificateServiceTests
    {
        readonly CertificateService _certificateService;
        IOptionsSnapshot<AppSettingsOptions> _configuration;

        public CertificateServiceTests()
        {
            //TODO: Init mail config here for full testing
            _certificateService = new CertificateService(_configuration);
        }

        [Theory]
        [InlineData("test")]
        public void Encrypt(string plainText)
        {

        }
    }
}
