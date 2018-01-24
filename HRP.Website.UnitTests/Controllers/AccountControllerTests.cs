namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Claims;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Criteria.User;
    using Fanex.Data.Repository;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Primitives;
    using Models;
    using Models.Account;
    using NSubstitute;
    using NSubstitute.Core;
    using Services;
    using Website.Controllers;
    using Xunit;

    public class AccountControllerTests
    {
        private const string SkipOtpCheck = "skip";
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ITwoFactorAuthenticatorService twoFactorAuthService;
        private readonly IUserService userService;
        private AccountController accountController;

        public AccountControllerTests()
        {
            sessionService = FakeSessionService();

            baseService = FakeBaseService();

            twoFactorAuthService = FakeTwoFactorAuthenticatorService();

            userService = FakeUserService();

            accountController = new AccountController(
                sessionService,
                baseService,
                twoFactorAuthService,
                userService);
        }

        #region Helper

        public static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();
            service.ExecuteNonQueryAsync(Arg.Any<ICriteria>()).Returns(new CommandResult());

            return service;
        }

        public static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.Username = "hunter.tran.hrp";
            service.UserId = 23;

            return service;
        }

        public static ITwoFactorAuthenticatorService FakeTwoFactorAuthenticatorService()
        {
            var service = Substitute.For<ITwoFactorAuthenticatorService>();
            service.ValidateOTPCode(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            service.ValidateOTPCode(Arg.Any<string>(), SkipOtpCheck).Returns(false);

            return service;
        }

        public static IUserService FakeUserService()
        {
            var service = Substitute.For<IUserService>();

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

            service.GetUserByUsername(Arg.Any<string>()).Returns(user);
            service.UpdateOTPStatus(Arg.Any<int>(), Arg.Any<bool>()).Returns(true);

            return service;
        }

        public AccountController FakeAccountController(bool isAuthenticated)
        {
            var authService = Substitute.For<IAuthenticationService>();

            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IAuthenticationService)).Returns(authService);

            var session = Substitute.For<ISession>();
            session.Id.Returns("sessionId");

            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProvider,
                    Session = session,
                    Connection =
                    {
                        RemoteIpAddress = new IPAddress(0x2414188f)
                    },
                    Request =
                    {
                        Headers =
                        {
                            new KeyValuePair<string, StringValues>("User-Agent", "test-user-agent")
                        }
                    }
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
                .Returns(x => new RedirectToActionResult(x.ArgAt<string>(0), x.ArgAt<string>(1), null));

            controller.RedirectToAction(Arg.Any<string>())
                .Returns(x => new RedirectToActionResult(x.ArgAt<string>(0), null, null));

            controller.RedirectToAction(Arg.Any<string>(), Arg.Any<object>())
                .Returns(x => new RedirectToActionResult(x.ArgAt<string>(0), null, x.ArgAt<object>(1)));

            controller.View(Arg.Any<string>(), Arg.Any<object>()).Returns(x => new ViewResult
            {
                ViewName = x.ArgAt<string>(0)
            });

            controller.View(Arg.Any<string>()).Returns(x => new ViewResult
            {
                ViewName = x.ArgAt<string>(0)
            });

            controller.Forbid().Returns(new ForbidResult());

            controller.Ok().Returns(new OkResult());

            return controller;
        }

        #endregion Helper

        [Fact]
        public void ChangePassword_AnyModel_ReturnJsonResult()
        {
            //Act
            var actual = accountController.ChangePassword(new ChangePasswordViewModel());

            //Assert
            Assert.IsType<JsonResult>(actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void UpdateProfile_AnyModel_ReturnJsonResult(int code)
        {
            //Arrange
            baseService.ExecuteNonQuery(Arg.Any<UpdateUserProfileCriteria>()).Returns(new CommandResult { Code = code, Message = string.Empty });

            //Act
            var actual = accountController.UpdateProfile(new UserProfileViewModel());

            //Assert
            Assert.IsType<JsonResult>(actual);
        }

        [Theory]
        [InlineData(0, 0, "Your password has been successfully changed.")]
        [InlineData(-1, -1, "Update failed")]
        public void ChangePassword_CorrectOldPasswordAndUpdatePassword_ReturnJsonResult(int codeCommandResult, int expectedCode, string expectedMessage)
        {
            //Arrange
            userService.IsCorrectPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            baseService.ExecuteNonQuery(Arg.Any<UpdateUserPasswordCriteria>()).Returns(new CommandResult { Code = codeCommandResult });
            var viewModel = new ChangePasswordViewModel
            {
                OldPassword = string.Empty,
                Password = "test_password"
            };

            //Act
            var actual = accountController.ChangePassword(viewModel);
            var jsonResult = Assert.IsType<JsonResult>(actual);

            //Assert
            Assert.Equal(expectedCode, (int)jsonResult.Value.GetType().GetProperty("code").GetValue(jsonResult.Value));
            Assert.Equal(expectedMessage, (string)jsonResult.Value.GetType().GetProperty("message").GetValue(jsonResult.Value));
        }

        [Fact]
        public void ChangePassword_IncorrectOldPassword_ReturnJsonResult()
        {
            //Arrange
            userService.IsCorrectPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
            var viewModel = new ChangePasswordViewModel
            {
                OldPassword = string.Empty,
                Password = "test_password"
            };

            //Act
            var actual = accountController.ChangePassword(viewModel) as JsonResult;
            var jsonResult = Assert.IsType<JsonResult>(actual);

            //Assert
            Assert.Equal(-2, (int)jsonResult.Value.GetType().GetProperty("code").GetValue(jsonResult.Value));
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
            // Test
            var actual = accountController.RegisterOTPNotification(sessionService.Username);

            var viewResult = Assert.IsType<ViewResult>(actual);

            // Assert
            Assert.IsType<ViewResult>(actual);
            Assert.Equal("RegisterOTPNotification", viewResult.ViewName);
            Assert.Equal(sessionService.Username, viewResult.Model.ToString());
        }

        [Fact]
        public void ResetOtpDevice_None_ReturnRedirectToActionResult()
        {
            var actual = accountController.ResetOtpDevice();
            Assert.IsType<RedirectToActionResult>(actual);
        }

        [Fact]
        public void SetupOtp_EmptyOtp_RedirectToLogin()
        {
            // Arrage
            var user = userService.GetUserByUsername(sessionService.Username);
            baseService.GetDataEntity<User>(Arg.Any<ICriteria>()).Returns(user);

            // Test
            var actual = accountController.SetupOtp(string.Empty);

            // Assert
            Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Login", ((RedirectToActionResult)actual).ActionName);
        }

        [Fact]
        public void SetupOtp_OtpInputed_RedirectToView()
        {
            // Arrage
            var user = userService.GetUserByUsername(sessionService.Username);
            baseService.GetDataEntity<User>(Arg.Any<ICriteria>()).Returns(user);

            // Test
            var actual = accountController.SetupOtp("test");
            var viewModel = ((ViewResult)actual).Model;

            // Assert
            Assert.IsType<ViewResult>(actual);
            Assert.Equal("SetupOtp", ((ViewResult)actual).ViewName);
            Assert.NotNull(viewModel);
        }

        [Fact]
        public void SetupOtpAfterReset_NullUser_RedirectToLogin()
        {
            // Arrange
            var user = userService.GetUserByUsername(sessionService.Username);
            baseService.GetDataEntity<User>(Arg.Any<ICriteria>()).Returns(user);

            // Test
            var actual = accountController.SetupOtpAfterReset(string.Empty);

            // Assert
            Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Login", ((RedirectToActionResult)actual).ActionName);
        }

        [Fact]
        public void SetupOtpAfterReset_ValidUser_ReturnIndex()
        {
            // Arrange
            var user = userService.GetUserByUsername(sessionService.Username);
            baseService.GetDataEntity<User>(Arg.Any<ICriteria>()).Returns(user);

            // Test
            var actual = accountController.SetupOtpAfterReset("sampletoken");

            // Assert
            Assert.IsType<ViewResult>(actual);
            Assert.Equal("Index", ((ViewResult)actual).ViewName);
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

        [Fact]
        public void VerifyOTP_AuthenticatedUser_ReturnIndex()
        {
            // Arrange
            sessionService.Username = "test";
            accountController = FakeAccountController(true);

            // Test
            var actual = accountController.VerifyOTP();

            // Assert
            Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", ((RedirectToActionResult)actual).ActionName);
        }

        [Fact]
        public void VerifyOTP_NullUserName_ReturnLogin()
        {
            // Arrange
            sessionService.Username = null;
            accountController = FakeAccountController(false);

            // Test
            var actual = accountController.VerifyOTP();

            // Assert
            Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Login", ((RedirectToActionResult)actual).ActionName);
        }

        [Fact]
        public void VerifyOTP_CorrectUsername_ReturnVerifyOTP()
        {
            // Arrange
            sessionService.Username = "hunter.tran.hrp";
            accountController = FakeAccountController(false);

            // Test
            var actual = accountController.VerifyOTP();

            // Assert
            Assert.IsType<ViewResult>(actual);
            Assert.Equal("VerifyOTP", ((ViewResult)actual).ViewName);
        }

        [Fact]
        public void VerifyOTP_OtpInputedAuthenticatedUser_ReturnIndex()
        {
            // Arrange
            sessionService.Username = "test";
            accountController = FakeAccountController(true);

            // Test
            var actual = accountController.VerifyOTP(new TwoFactorAuthenticatorViewModel()).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", ((RedirectToActionResult)actual).ActionName);
        }

        [Fact]
        public void VerifyOTP_OtpInputedNullUser_RedirectToLogin()
        {
            // Arrange
            sessionService.Username = null;
            accountController = FakeAccountController(false);
            userService.GetUserByUsername(Arg.Any<string>()).Returns(null as Func<CallInfo, User>);

            // Test
            var actual = accountController.VerifyOTP(new TwoFactorAuthenticatorViewModel()).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Login", ((RedirectToActionResult)actual).ActionName);
        }

        [Fact]
        public void VerifyOTP_OtpInputedInvalidOtp_ReturnIndex()
        {
            // Arrange
            sessionService.Username = "test";
            accountController = FakeAccountController(false);
            var viewModel = new TwoFactorAuthenticatorViewModel { OTPCode = SkipOtpCheck };

            // Test
            var actual = accountController.VerifyOTP(viewModel).Result;

            // Assert
            Assert.IsType<ViewResult>(actual);
            Assert.Equal("VerifyOTP", ((ViewResult)actual).ViewName);
        }

        [Fact]
        public void VerifyOTP_OtpInputedValidOtp_ReturnIndex()
        {
            // Arrange
            sessionService.Username = "test";
            accountController = FakeAccountController(false);
            var viewModel = new TwoFactorAuthenticatorViewModel { OTPCode = "test" };

            // Test
            var actual = accountController.VerifyOTP(viewModel).Result;

            // Assert
            Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", ((RedirectToActionResult)actual).ActionName);
        }

        [Fact]
        public void VerifySetupOTP_NullUser_RedirectToLogin()
        {
            // Arrange
            sessionService.Username = null;
            accountController = FakeAccountController(false);
            userService.GetUserByUsername(Arg.Any<string>()).Returns(null as Func<CallInfo, User>);

            // Test
            var actual = accountController.VerifySetupOTP(new SetupOtpViewModel());

            // Assert
            Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Login", ((RedirectToActionResult)actual).ActionName);
        }

        [Fact]
        public void VerifySetupOTP_InvalidOtp_ForbidResult()
        {
            // Arrange
            sessionService.Username = "test";
            accountController = FakeAccountController(false);
            var viewModel = new SetupOtpViewModel { Otp = SkipOtpCheck };

            // Test
            var actual = accountController.VerifySetupOTP(viewModel);

            // Assert
            Assert.IsType<ForbidResult>(actual);
        }

        [Fact]
        public void VerifySetupOTP_ValidOtp_OkResult()
        {
            // Arrange
            sessionService.Username = "test";
            accountController = FakeAccountController(false);
            var viewModel = new SetupOtpViewModel { Otp = "test" };

            // Test
            var actual = accountController.VerifySetupOTP(viewModel);

            // Assert
            Assert.IsType<OkResult>(actual);
        }
    }
}