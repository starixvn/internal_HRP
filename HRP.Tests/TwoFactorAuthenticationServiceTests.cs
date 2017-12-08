namespace HRP.Tests.Services
{
    using HRP.Services;
    using Xunit;

    public class TwoFactorAuthenticationServiceTests
    {
        readonly TwoFactorAuthenticatorService _twoFactorAuthenticationService;

        public TwoFactorAuthenticationServiceTests()
        {
            _twoFactorAuthenticationService = new TwoFactorAuthenticatorService();
        }

        [Theory]
        [InlineData("admine","1234")]
        public void GenerateQrImageUrl(string username, string secretKey)
        {
            //var result = _twoFactorAuthenticationService.GenerateQrImageUrl(username, secretKey);

            //Assert.True(!string.IsNullOrEmpty(result));
        }

        [Theory]
        [InlineData("admine", "1234")]
        public void GenerateSetupCode(string username, string secretKey)
        {
            //var result = _twoFactorAuthenticationService.GenerateSetupCode(username, secretKey);

            //Assert.True(!string.IsNullOrEmpty(result));
        }

        [Theory]
        [InlineData("admine", "1234", "123456")]
        public void ValidateOTPCode(string username, string secretKey, string otpCode)
        {
            //var result = _twoFactorAuthenticationService.ValidateOTPCode(username, secretKey, otpCode);

            //Assert.True(result);
        }
    }
}
