namespace HRP.Services.UnitTests
{
    using HRP.Services;

    public class TwoFactorAuthenticationServiceTests
    {
        private readonly TwoFactorAuthenticatorService _twoFactorAuthenticationService;

        public TwoFactorAuthenticationServiceTests()
        {
            _twoFactorAuthenticationService = new TwoFactorAuthenticatorService();
        }
    }
}