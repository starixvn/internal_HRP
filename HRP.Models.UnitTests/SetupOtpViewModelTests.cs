namespace HRP.Models.UnitTests
{
    using HRP.Models.Account;
    using Xunit;

    public class SetupOtpViewModelTests
    {
        [Fact]
        public void QrCodeText_AlwaysRetunAsExpected()
        {
            var model = new SetupOtpViewModel
            {
                TextCode = "TextCode",
                AppName = "AppName",
                Username = "Username"
            };

            Assert.Equal("otpauth://totp/AppName:Username?secret=TextCode&issuer=AppName", model.QrCodeText);
        }
    }
}
