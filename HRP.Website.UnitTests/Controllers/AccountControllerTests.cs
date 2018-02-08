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
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Primitives;
    using Models;
    using Models.Account;
    using NSubstitute;
    using NSubstitute.Core;
    using Services;
    using Services.Interfaces;
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

        #region ChangePassword

        [Fact]
        public void ChangePassword_ModelIsInvalid_ReturnAsExpectedJsonResult()
        {
            // Arrange
            accountController.ModelState.AddModelError("error", "error");

            // Act
            var actual = accountController.ChangePassword(new ChangePasswordViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.Collection(
                errors,
                kvp => { Assert.Equal("code", kvp.Key); Assert.Equal(-1, kvp.Value); },
                kvp => { Assert.Equal("message", kvp.Key); Assert.Equal("Update failed", kvp.Value); }
            );
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

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.Collection(
                errors,
                kvp => { Assert.Equal("code", kvp.Key); Assert.Equal(-2, kvp.Value); },
                kvp => { Assert.Equal("message", kvp.Key); Assert.Equal("The old password you entered is incorrect.", kvp.Value); }
            );
        }

        [Theory]
        [InlineData(0, 0, "Your password has been successfully changed.")]
        [InlineData(-1, -1, "Update failed")]
        public void ChangePassword_CorrectOldPasswordAndUpdatePassword_ReturnJsonResult(
            int codeCommandResult,
            int expectedCode,
            string expectedMessage)
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

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.Collection(
                errors,
                kvp => { Assert.Equal("code", kvp.Key); Assert.Equal(expectedCode, kvp.Value); },
                kvp => { Assert.Equal("message", kvp.Key); Assert.Equal(expectedMessage, kvp.Value); }
            );
        }

        #endregion ChangePassword

        [Fact]
        public void RegisterOTPNotification_WhenCalled_ReturnViewResult()
        {
            // Act
            var actual = accountController.RegisterOTPNotification(sessionService.Username);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsType<ViewResult>(actual);
            Assert.Equal("RegisterOTPNotification", viewResult.ViewName);
            Assert.Equal(sessionService.Username, viewResult.Model.ToString());
        }

        #region ResetOtpDevice

        [Fact]
        public void ResetOtpDevice_WhenCalled_ReturnRedirectToActionResult()
        {
            // Arrange
            var token = "token";
            userService.GenerateTokenOTPRegister(Arg.Any<string>()).Returns(token);

            // Act
            var actual = accountController.ResetOtpDevice();

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Account", viewResult.ControllerName);
            Assert.Equal("SetupOtpAfterReset", viewResult.ActionName);
            Assert.Collection(
                viewResult.RouteValues,
                kvp => { Assert.Equal("token", kvp.Key); Assert.Equal(token, kvp.Value); }
            );
        }

        #endregion ResetOtpDevice

        #region SetupOtp

        [Fact]
        public void SetupOtp_EmptyOtp_RedirectToLogin()
        {
            // Act
            var actual = accountController.SetupOtp(string.Empty);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Auth", viewResult.ControllerName);
            Assert.Equal("Login", viewResult.ActionName);
        }

        [Fact]
        public void SetupOtp_UserIsNotExisted_RedirectToLogin()
        {
            // Arrage
            userService.GetUserByTokenOTPRegister(Arg.Any<string>()).Returns(x => null);

            // Act
            var actual = accountController.SetupOtp("token");

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Auth", viewResult.ControllerName);
            Assert.Equal("Login", viewResult.ActionName);
        }

        [Fact]
        public void SetupOtp_UserIsExisted_ReturnAsExpectedViewResult()
        {
            // Arrage
            var user = new User { Username = "tim.trinh" };
            userService.GetUserByTokenOTPRegister(Arg.Any<string>()).Returns(user);

            // Test
            var actual = accountController.SetupOtp("test");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Equal("SetupOtp", viewResult.ViewName);
            Assert.NotNull(viewResult.Model);
        }

        #endregion SetupOtp

        #region SetupOtpAfterReset

        [Fact]
        public void SetupOtpAfterReset_EmptyToken_RedirectToLogin()
        {
            // Act
            var actual = accountController.SetupOtpAfterReset(string.Empty);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Auth", viewResult.ControllerName);
            Assert.Equal("Login", viewResult.ActionName);
        }

        [Fact]
        public void SetupOtpAfterReset_UserIsNotExisted_RedirectToLogin()
        {
            // Arrange
            userService.GetUserByTokenOTPRegister(Arg.Any<string>()).Returns(x => null);

            // Act
            var actual = accountController.SetupOtpAfterReset("token");

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Auth", viewResult.ControllerName);
            Assert.Equal("Login", viewResult.ActionName);
        }

        [Fact]
        public void SetupOtpAfterReset_UserIsExisted_ReturnAsExpectedViewResult()
        {
            // Arrange
            var user = new User { Username = "tim.trinh" };
            userService.GetUserByTokenOTPRegister(Arg.Any<string>()).Returns(user);

            // Act
            var actual = accountController.SetupOtpAfterReset("token");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.NotNull(viewResult.Model);
        }

        #endregion SetupOtpAfterReset

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
            // Arrange
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

            // Act
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

            // Act
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

            // Act
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

            // Act
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

            // Act
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

            // Act
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

            // Act
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

            // Act
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

            // Act
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

            // Act
            var actual = accountController.VerifySetupOTP(viewModel);

            // Assert
            Assert.IsType<OkResult>(actual);
        }
    }
}