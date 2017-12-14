namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Threading.Tasks;
    using Core.Enumerations;
    using Core.Services;
    using Fanex.Data.Repository;
    using HRP.Models;
    using HRP.Models.Account;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using NSubstitute.Core;
    using Services;
    using Website.Controllers;
    using Xunit;

    public class AccountControllerTests
    {
        private readonly AccountController _accountController;
        private readonly IBaseService _baseService;
        private readonly ISessionService _sessionService;
        private readonly IUserService _userService;

        private const string SkipOtpCheck = "skip";

        public AccountControllerTests()
        {
            _sessionService = FakeSessionService();
            
            _baseService = Substitute.For<IBaseService>();

            var twoFactorAuthService = FakeTwoFactorAuthenticatorService();
            
            _userService = FakeUserService();

            _accountController = new AccountController(
                _sessionService,
                _baseService,
                twoFactorAuthService,
                _userService);
        }

        public static ISessionService FakeSessionService()
        {
            var sessionService = Substitute.For<ISessionService>();
            sessionService.Username = "hunter.tran.hrp";
            sessionService.UserId = 23;

            return sessionService;
        }

        public static ITwoFactorAuthenticatorService FakeTwoFactorAuthenticatorService()
        {
            var twoFactorAuthService = Substitute.For<ITwoFactorAuthenticatorService>();
            twoFactorAuthService.ValidateOTPCode(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            twoFactorAuthService.ValidateOTPCode(Arg.Any<string>(), SkipOtpCheck).Returns(false);

            return twoFactorAuthService;
        }

        public static IUserService FakeUserService()
        {
            var userService = Substitute.For<IUserService>();

            var user = new User
            {
                UserId = 23,
                Username = "hunter.tran.hrp",
                FullName = "Tuấn Trần",
                Phone = "0963230397",
                Email = "hunter.tran@nexcel.vn",
                Password = "$2a$10$W6FBVhwpAOXt2u3FNoU9h.nuipn0bbvMW6oO8w9i015MzOJgnGlsm",
                IsActive = true,
                LastLoginDate = DateTime.Parse("2017-12-13 13:51:59.087"),
                IsOTPRegistered = true,
                SecretKey = "4b7be5aea6fd",
                CreatedBy = 1,
                CreatedDate = DateTime.Parse("2017-12-11 09:04:56.543"),
                IsSystemAdmin = false,
            };

            userService.GetUserByUsername(Arg.Any<string>()).Returns(user);
            userService.UpdateOTPStatus(Arg.Any<int>(), Arg.Any<bool>()).Returns(true);

            return userService;
        }

        [Theory]
        [InlineData(AccountTab.Profile)]
        [InlineData(AccountTab.ChangePassword)]
        [InlineData(AccountTab.Otp)]
        public void Index_TabSelected_ReturnViewResultWithActivePanelEqualSelectedTab(AccountTab tab)
        {
            // Act
            var actual = _accountController.Index(tab);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<AccountViewModel>(viewResult.Model);

            Assert.NotNull(viewModel);
            Assert.Equal(tab, viewModel.ActivePanel);
        }

        [Fact]
        public void RegisterOTPNotification_None_ReturnViewResult()
        {
            var actual = _accountController.RegisterOTPNotification();

            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void ResetOtpDevice_None_ReturnRedirectToActionResult()
        {
            var actual = _accountController.ResetOtpDevice();
            Assert.IsType<RedirectToActionResult>(actual);
        }

        [Theory]
        [InlineData("", typeof(BadRequestResult))]
        [InlineData("sampletoken", typeof(ViewResult))]
        public void SetupOtp_NullOrValid_BadRequestOrOkResult(string token, Type type)
        {
            //Arrange
            var user = _userService.GetUserByUsername(_sessionService.Username);
            _baseService.GetDataEntity<User>(Arg.Any<ICriteria>()).Returns(user);

            var actual = _accountController.SetupOtp(token);
            Assert.IsType(type, actual);
        }

        [Theory]
        [InlineData(null, typeof(RedirectToActionResult))]
        [InlineData("test", typeof(ViewResult))]
        public void VerifyOTP_None_ReturnViewResult(string username, Type type)
        {
            _sessionService.Username = username;
            var actual = _accountController.VerifyOTP();
            Assert.IsType(type, actual);
        }

        [Fact]
        public void VerifyOTP_OtpInputed_ReturnViewResult()
        {
            TwoFactorAuthenticatorViewModel viewModel = new TwoFactorAuthenticatorViewModel();
            var actual = _accountController.VerifyOTP(viewModel);
            Assert.IsType<Task<IActionResult>>(actual);

            viewModel.OTPCode = SkipOtpCheck;
            actual = _accountController.VerifyOTP(viewModel);
            Assert.IsType<Task<IActionResult>>(actual);

            _userService.GetUserByUsername(Arg.Any<string>()).Returns(null as Func<CallInfo, User>);
            actual = _accountController.VerifyOTP(viewModel);
            Assert.IsType<Task<IActionResult>>(actual);
        }

        [Theory]
        [InlineData(true, typeof(OkResult))]
        [InlineData(false, typeof(BadRequestResult))]
        public void VerifySetupOTP_None_ReturnOkResult(bool isOTPValid, Type type)
        {
            SetupOtpViewModel viewModel = new SetupOtpViewModel
            {
                Username = _sessionService.Username,
                Otp = isOTPValid ? "sample" : SkipOtpCheck
            };

            var actual = _accountController.VerifySetupOTP(viewModel);
            Assert.IsType(type, actual);

            _userService.GetUserByUsername(Arg.Any<string>()).Returns(null as Func<CallInfo, User>);
            actual = _accountController.VerifySetupOTP(viewModel);
            Assert.IsType<RedirectToActionResult>(actual);
        }
    }
}
