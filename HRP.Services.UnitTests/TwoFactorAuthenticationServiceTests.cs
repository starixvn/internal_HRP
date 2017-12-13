namespace HRP.Services.UnitTests
{
    using Core.Model;
    using Microsoft.Extensions.Options;
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

            var otpConfig = Substitute.For<IOptionsSnapshot<OtpConfig>>();
            otpConfig.Value.Returns(config);

            _twoFactorAuthenticationService = new TwoFactorAuthenticatorService(otpConfig);
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