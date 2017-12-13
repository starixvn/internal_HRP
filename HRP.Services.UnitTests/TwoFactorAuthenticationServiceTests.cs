namespace HRP.Services.UnitTests
{
    using Core.Model;
    using Microsoft.Extensions.Options;
    using Moq;
    using NSubstitute;
    using Services;
    using Xunit;

    public class TwoFactorAuthenticationServiceTests
    {
        private readonly TwoFactorAuthenticatorService _twoFactorAuthenticationService;

        public TwoFactorAuthenticationServiceTests()
        {
            var config = new OtpConfig
            {
                AppName = "HRP",
                OtpExpireTime = 0
            };

            var otpConfig = new Mock<IOptionsSnapshot<OtpConfig>>();
            otpConfig.Setup(m => m.Value).Returns(config);
            
            _twoFactorAuthenticationService = new TwoFactorAuthenticatorService(otpConfig.Object);
        }

        [Theory]
        [InlineData("hunter.tran.hrp", "d6555591fa48", "MQ3DKNJVGU4TCZTBGQ4A")]
        public void SetupGoogleAuthInfo(string username, string secretKey, string generatedKey)
        {
            var setupCode = _twoFactorAuthenticationService.SetupGoogleAuthInfo(username, secretKey);
            Assert.Equal(setupCode, generatedKey);
        }

        [Theory]
        [InlineData("d6555591fa48", "123456")]
        public void ValidateOTPCode(string secretKey, string otpCode)
        {
            var isValid = _twoFactorAuthenticationService.ValidateOTPCode(secretKey, otpCode);

            Assert.False(isValid);
        }
    }
}