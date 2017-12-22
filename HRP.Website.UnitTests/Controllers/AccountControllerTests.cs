namespace HRP.Website.UnitTests.Controllers
{
    using Core.Enumerations;
    using Core.Services;
    using Fanex.Data.Repository;
    using HRP.Core.Model;
    using HRP.Criteria.User;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Account;
    using NSubstitute;
    using NSubstitute.Core;
    using Services;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Website.Controllers;
    using Xunit;

    public class AccountControllerTests
    {
        private AccountController accountController;
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly IUserService userService;
        private readonly ITwoFactorAuthenticatorService twoFactorAuthService;

        private const string SkipOtpCheck = "skip";

        public AccountControllerTests()
        {
            sessionService = FakeSessionService();

            baseService = Substitute.For<IBaseService>();

            twoFactorAuthService = FakeTwoFactorAuthenticatorService();

            userService = FakeUserService();

            accountController = new AccountController(
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

        public static ITwoFactorAuthenticatorService FakeTwoFactorAuthenticatorService()
        {
            var twoFactorAuthService = Substitute.For<ITwoFactorAuthenticatorService>();
            twoFactorAuthService.ValidateOTPCode(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            twoFactorAuthService.ValidateOTPCode(Arg.Any<string>(), SkipOtpCheck).Returns(false);

            return twoFactorAuthService;
        }

        [Fact]
        public void UpdateProfile_InvalidModel_ReturnErrors()
        {
            // Arrange
            var model = new UserProfileViewModel();
            accountController.ModelState.AddModelError("", "Error");

            // Act
            var actual = accountController.UpdateProfile(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotEmpty(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value) as IEnumerable);
        }

        [Fact]
        public void UpdateProfile_ValidModel_ReturnSuccessfully()
        {
            var model = new UserProfileViewModel();
            baseService.ExecuteNonQuery(Arg.Any<UpdateUserProfileCriteria>()).Returns(new CommandResult { Code = 0, Message = string.Empty });

            // Act
            var actual = accountController.UpdateProfile(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
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

        public AccountController FakeAccountController(bool isAuthenticated)
        {
            var authService = Substitute.For<IAuthenticationService>();
            authService.SignInAsync(
                    Arg.Any<HttpContext>(),
                    Arg.Any<string>(),
                    Arg.Any<ClaimsPrincipal>(),
                    Arg.Any<AuthenticationProperties>())
                .Returns(Task.FromResult((object)null));

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IAuthenticationService)).Returns(authService);

            ControllerContext controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProvider
                }
            };

            controllerContext.HttpContext.User = isAuthenticated
                ? new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>(),
                    CookieAuthenticationDefaults.AuthenticationScheme))
                : new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()));

            var controller =
                Substitute.For<AccountController>(sessionService, baseService, twoFactorAuthService, userService);

            controller.ControllerContext = controllerContext;

            controller.RedirectToAction(Arg.Any<string>(), Arg.Any<string>())
                .Returns(new RedirectToActionResult("test", "test", "test"));
            controller.RedirectToAction(Arg.Any<string>()).Returns(new RedirectToActionResult("test", "test", "test"));
            controller.RedirectToAction(Arg.Any<string>(), Arg.Any<object>())
                .Returns(new RedirectToActionResult("test", "test", new { test = "test" }));

            controller.View(Arg.Any<string>(), Arg.Any<object>()).Returns(new ViewResult());
            controller.View(Arg.Any<string>()).Returns(new ViewResult());

            return controller;
        }

        [Theory]
        [InlineData(AccountTab.Profile)]
        [InlineData(AccountTab.ChangePassword)]
        [InlineData(AccountTab.Otp)]
        public void Index_TabSelected_ReturnViewResultWithActivePanelEqualSelectedTab(AccountTab tab)
        {
            // Act
            var actual = accountController.Index(tab);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<AccountViewModel>(viewResult.Model);

            Assert.NotNull(viewModel);
            Assert.Equal(tab, viewModel.ActivePanel);
        }

        [Fact]
        public void RegisterOTPNotification_None_ReturnViewResult()
        {
            var actual = accountController.RegisterOTPNotification(sessionService.Username);

            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void ResetOtpDevice_None_ReturnRedirectToActionResult()
        {
            var actual = accountController.ResetOtpDevice();
            Assert.IsType<RedirectToActionResult>(actual);
        }

        [Theory]
        [InlineData("", typeof(RedirectToActionResult))]
        [InlineData("sampletoken", typeof(ViewResult))]
        public void SetupOtp_NullOrValid_RedirectOrView(string token, Type type)
        {
            //Arrange
            var user = userService.GetUserByUsername(sessionService.Username);
            baseService.GetDataEntity<User>(Arg.Any<ICriteria>()).Returns(user);

            var actual = accountController.SetupOtp(token);
            Assert.IsType(type, actual);
        }

        [Theory]
        [InlineData("", typeof(RedirectToActionResult))]
        [InlineData("sampletoken", typeof(ViewResult))]
        public void SetupOtpAfterReset_NullOrValid_LoginOrAccountIndexResult(string token, Type type)
        {
            //Arrange
            var user = userService.GetUserByUsername(sessionService.Username);
            baseService.GetDataEntity<User>(Arg.Any<ICriteria>()).Returns(user);

            var actual = accountController.SetupOtpAfterReset(token);
            Assert.IsType(type, actual);
        }

        [Theory]
        [InlineData(null, false, typeof(RedirectToActionResult))]
        [InlineData(null, true, typeof(RedirectToActionResult))]
        [InlineData("test", false, typeof(ViewResult))]
        [InlineData("test", true, typeof(RedirectToActionResult))]
        public void VerifyOTP_None_ReturnViewResult(string username, bool isAuthenticated, Type type)
        {
            // Arrange
            sessionService.Username = username;
            accountController = FakeAccountController(isAuthenticated);

            // Test
            var actual = accountController.VerifyOTP();

            // Assert
            Assert.IsType(type, actual);
        }

        [Theory]
        [InlineData(true, "test", true, typeof(RedirectToActionResult))]
        [InlineData(false, "test", true, typeof(RedirectToActionResult))]
        [InlineData(false, "test", false, typeof(RedirectToActionResult))]
        [InlineData(true, "test", false, typeof(RedirectToActionResult))]
        [InlineData(false, SkipOtpCheck, true, typeof(ViewResult))]
        [InlineData(false, SkipOtpCheck, false, typeof(RedirectToActionResult))]
        [InlineData(true, SkipOtpCheck, true, typeof(RedirectToActionResult))]
        public void VerifyOTP_OtpInputed_ReturnViewResult(bool isAuthenticated, string otp, bool isUserExist, Type type)
        {
            // Arrange
            sessionService.UserId = 1;

            accountController = FakeAccountController(isAuthenticated);
            var viewModel = new TwoFactorAuthenticatorViewModel
            {
                OTPCode = otp
            };

            if (!isUserExist)
            {
                userService.GetUserByUsername(Arg.Any<string>()).Returns(null as Func<CallInfo, User>);
            }

            // Test
            var actual = accountController.VerifyOTP(viewModel);

            // Assert
            Assert.IsType(type, actual.Result);
        }

        [Theory]
        [InlineData(true, true, typeof(OkResult))]
        [InlineData(true, false, typeof(ForbidResult))]
        [InlineData(false, true, typeof(RedirectToActionResult))]
        public void VerifySetupOTP_None_ReturnOkResult(bool isUserExist, bool isOTPValid, Type type)
        {
            SetupOtpViewModel viewModel = new SetupOtpViewModel
            {
                Username = sessionService.Username,
                Otp = isOTPValid ? "sample" : SkipOtpCheck
            };

            if (!isUserExist)
            {
                userService.GetUserByUsername(Arg.Any<string>()).Returns(null as Func<CallInfo, User>);
            }

            var actual = accountController.VerifySetupOTP(viewModel);
            Assert.IsType(type, actual);
        }
    }
}
