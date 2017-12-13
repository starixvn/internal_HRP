namespace HRP.Website.UnitTests.Controllers
{
    using Core.Services;
    using NSubstitute;
    using Services;
    using Website.Controllers;

    public class AccountControllerTests
    {
        private AccountController _accountController;

        private readonly ISessionService _sessionService;

        private readonly IBaseService _baseService;

        private readonly ITwoFactorAuthenticatorService _twoFactorAuthService;

        private readonly IUserService _userService;

        public AccountControllerTests()
        {
            _sessionService = Substitute.For<ISessionService>();
            _sessionService.Username = "hunter.tran.hrp";

            _baseService = Substitute.For<IBaseService>();
            _twoFactorAuthService = Substitute.For<ITwoFactorAuthenticatorService>();
            _userService = Substitute.For<IUserService>();

            _accountController = new AccountController(
                _sessionService,
                _baseService,
                _twoFactorAuthService,
                _userService);
        }


    }
}
