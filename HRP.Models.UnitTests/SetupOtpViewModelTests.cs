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
                SystemName = "SystemName",
                Username = "Username"
            };

            Assert.Equal("otpauth://totp/{SystemName}:{Username}?secret={TextCode}&issuer={SystemName}", model.QrCodeText);
        }
    }
}
