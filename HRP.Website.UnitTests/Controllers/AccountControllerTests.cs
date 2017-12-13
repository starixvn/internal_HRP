namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using Core.Enumerations;
    using Core.Services;
    using HRP.Models;
    using HRP.Models.Account;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Services;
    using Website.Controllers;
    using Xunit;

    public class AccountControllerTests
    {
        private readonly AccountController _accountController;

        public AccountControllerTests()
        {
            var sessionService = FakeSessionService();
            var baseService = Substitute.For<IBaseService>();
            var twoFactorAuthService = Substitute.For<ITwoFactorAuthenticatorService>();
            var userService = FakeUserService();

            _accountController = new AccountController(
                sessionService,
                baseService,
                twoFactorAuthService,
                userService);
        }

        public static ISessionService FakeSessionService()
        {
            var sessionService = Substitute.For<ISessionService>();
            sessionService.Username = "hunter.tran.hrp";
            sessionService.UserId = 23;

            return sessionService;
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

        [Fact]
        public void SetupOtp_None_RReturnRedirectToActionResult()
        {
            var resetActual = _accountController.ResetOtpDevice();
            Assert.IsType<RedirectToActionResult>(resetActual);

            ////string token = setupOtpResult.RouteValues["token"].ToString();

            ////var user = _userService.GetUserByUsername(_sessionService.Username);
            ////var oldSecretKey = user.SecretKey;

            ////// User's secret key changed here
            ////var actual = _accountController.SetupOtp(token);
            ////user = _userService.GetUserByUsername(_sessionService.Username);

            ////var viewResult = Assert.IsType<ViewResult>(actual);

            ////var viewModel = Assert.IsType<AccountViewModel>(viewResult.Model);

            ////Assert.Equal(AccountTab.Otp, viewModel.ActivePanel);
            ////Assert.False(viewModel.IsOTPRegistered);
            ////Assert.NotEqual(oldSecretKey, user.SecretKey);

            ////// Reset value
            ////_userService.ResetTokenOTPRegister(_sessionService.Username);
            ////_userService.UpdateSecretKey(_sessionService.Username, oldSecretKey);
        }

        [Fact]
        public void VerifyOTP_None_ReturnViewResult()
        {
            var actual = _accountController.VerifyOTP();

            Assert.IsType<ViewResult>(actual);
        }
    }
}
