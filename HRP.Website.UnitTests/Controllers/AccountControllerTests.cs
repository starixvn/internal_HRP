namespace HRP.Website.UnitTests.Controllers
{
    using Core.Services;
    using HRP.Models.Account;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Services;
    using Website.Controllers;
    using Xunit;

    public class AccountControllerTests
    {
        private readonly AccountController _accountController;

        private readonly ISessionService _sessionService;

        private readonly IBaseService _baseService;

        private readonly ITwoFactorAuthenticatorService _twoFactorAuthService;

        private readonly IUserService _userService;

        public AccountControllerTests()
        {
            _sessionService = Substitute.For<ISessionService>();
            _sessionService.Username = "hunter.tran.hrp";
            _sessionService.UserId = 23;

            _baseService = Substitute.For<IBaseService>();
            _twoFactorAuthService = Substitute.For<ITwoFactorAuthenticatorService>();
            _userService = Substitute.For<IUserService>();

            _accountController = new AccountController(
                _sessionService,
                _baseService,
                _twoFactorAuthService,
                _userService);
        }

        [Fact]
        public void Index()
        {
            var actual = _accountController.Index();

            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<AccountViewModel>(viewResult.Model);

            Assert.NotNull(viewModel);
        }

        [Fact]
        public void RegisterOTPNotification()
        {
            var actual = _accountController.RegisterOTPNotification();

            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void ResetOtpDevice()
        {
            var actual = _accountController.ResetOtpDevice();

            var viewResult = Assert.IsType<RedirectToActionResult>(actual);

            var user = _userService.GetUserByUsername(_sessionService.Username);

            Assert.NotNull(user);

            Assert.False(user.IsOTPRegistered);
            Assert.NotNull(viewResult.RouteValues["token"]);

            // Reset value
            _userService.UpdateOTPStatus(_sessionService.UserId, true);
            _userService.ResetTokenOTPRegister(_sessionService.Username);
        }

        [Fact]
        public void SetupOtp()
        {
            var resetActual = _accountController.ResetOtpDevice();

            var setupOtpResult = Assert.IsType<RedirectToActionResult>(resetActual);
            string token = setupOtpResult.RouteValues["token"].ToString();

            var user = _userService.GetUserByUsername(_sessionService.Username);
            var oldSecretKey = user.SecretKey;

            // User's secret key changed here
            var actual = _accountController.SetupOtp(token);

            var viewResult = Assert.IsType<ViewResult>(actual);


        }
    }
}
