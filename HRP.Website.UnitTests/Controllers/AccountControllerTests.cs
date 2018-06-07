namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Criteria.User;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.Primitives;
    using Models;
    using Models.Account;
    using NSubstitute;
    using Services;
    using Website.Controllers;
    using Xunit;

    public class AccountControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private ITwoFactorAuthenticatorService twoFactorAuthService;
        private IUserService userService;
        private IOptionsSnapshot<OtpConfig> otpConfig;
        private IRijndaelService rijndaelService;

        [Theory]
        [InlineData(AccountTab.Profile)]
        [InlineData(AccountTab.ChangePassword)]
        [InlineData(AccountTab.Otp)]
        public async Task Index_WhenCalled_ReturnAsExpectedViewResult(AccountTab tab)
        {
            // Arrange
            var username = "hunter.tran";
            var user = new User
            {
                FullName = "Tuấn Trần",
                Phone = "0963230397",
                Email = "hunter.tran@nexcel.vn"
            };
            var accountController = GetAccountController();
            sessionService.Username = username;
            userService.GetUserByUsernameAsync(username).Returns(user);

            // Act
            var actual = await accountController.Index(tab);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<AccountViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Collection(
                viewResult.ViewData,
                kvp =>
                {
                    Assert.Equal("Username", kvp.Key);
                    Assert.Equal(username, kvp.Value);
                });
            Assert.Equal(user.IsOTPRegistered, viewModel.IsOTPRegistered);
            Assert.Equal(tab, viewModel.ActivePanel);
            Assert.Null(viewModel.SetupOtpViewModel);
            Assert.Null(viewModel.ChangePasswordViewModel);
            Assert.Equal(user.FullName, viewModel.UserProfileViewModel.FullName);
            Assert.Equal(user.Email, viewModel.UserProfileViewModel.Email);
            Assert.Equal(user.Phone, viewModel.UserProfileViewModel.Phone);
        }

        #region ChangePassword

        [Fact]
        public async Task ChangePassword_ModelIsInvalid_ReturnAsExpectedJsonResult()
        {
            // Arrange
            var accountController = GetAccountController();
            accountController.ModelState.AddModelError("error", "error");

            // Act
            var actual = await accountController.ChangePassword(new ChangePasswordViewModel());

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.Collection(
                errors,
                kvp =>
                {
                    Assert.Equal("code", kvp.Key);
                    Assert.Equal(-1, kvp.Value);
                },
                kvp =>
                {
                    Assert.Equal("message", kvp.Key);
                    Assert.Equal("Update failed", kvp.Value);
                }
            );
        }

        [Fact]
        public async Task ChangePassword_IncorrectOldPassword_ReturnAsExpectedJsonResult()
        {
            //Arrange
            var accountController = GetAccountController();
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(new User());
            userService.IsCorrectPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
            var viewModel = new ChangePasswordViewModel
            {
                OldPassword = string.Empty,
                Password = "test_password"
            };

            // Act
            var actual = await accountController.ChangePassword(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.Collection(
                errors,
                kvp =>
                {
                    Assert.Equal("code", kvp.Key);
                    Assert.Equal(-2, kvp.Value);
                },
                kvp =>
                {
                    Assert.Equal("message", kvp.Key);
                    Assert.Equal("The old password you entered is incorrect.", kvp.Value);
                }
            );
        }

        [Theory]
        [InlineData(0, 0, "Your password has been successfully changed.")]
        [InlineData(-1, -1, "Update failed")]
        public async Task ChangePassword_CorrectOldPasswordAndUpdatePassword_ReturnAsExpectedJsonResult(
            int codeCommandResult,
            int expectedCode,
            string expectedMessage)
        {
            // Arrange
            var accountController = GetAccountController();
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(new User());
            userService.IsCorrectPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateUserPasswordCriteria>()).Returns(new CommandResult { Code = codeCommandResult });
            var viewModel = new ChangePasswordViewModel
            {
                OldPassword = string.Empty,
                Password = "test_password"
            };

            // Act
            var actual = await accountController.ChangePassword(viewModel);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value);
            Assert.Collection(
                errors,
                kvp =>
                {
                    Assert.Equal("code", kvp.Key);
                    Assert.Equal(expectedCode, kvp.Value);
                },
                kvp =>
                {
                    Assert.Equal("message", kvp.Key);
                    Assert.Equal(expectedMessage, kvp.Value);
                }
            );
        }

        #endregion ChangePassword

        [Fact]
        public void RegisterOTPNotification_WhenCalled_ReturnAsExpectedViewResult()
        {
            // Arrange
            var username = "hunter.tran";
            var accountController = GetAccountController();

            // Act
            var actual = accountController.RegisterOTPNotification(username);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Equal("RegisterOTPNotification", viewResult.ViewName);
            Assert.Equal(username, viewResult.Model.ToString());
        }

        [Fact]
        public async Task ResetOtpDevice_WhenCalled_ReturnRedirectToSetupOtpAfterReset()
        {
            // Arrange
            var token = "token";
            var accountController = GetAccountController();
            userService.GenerateTokenOTPRegisterAsync(Arg.Any<string>()).Returns(token);

            // Act
            var actual = await accountController.ResetOtpDevice();

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Account", viewResult.ControllerName);
            Assert.Equal("SetupOtpAfterReset", viewResult.ActionName);
            Assert.Collection(
                viewResult.RouteValues,
                kvp =>
                {
                    Assert.Equal("token", kvp.Key);
                    Assert.Equal(token, kvp.Value);
                }
            );
        }

        #region SetupOtp

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task SetupOtp_TokenIsInvalid_ReturnRedirectToLogout(string token)
        {
            // Arrange
            var accountController = GetAccountController();

            // Act
            var actual = await accountController.SetupOtp(token);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Auth", viewResult.ControllerName);
            Assert.Equal("Logout", viewResult.ActionName);
        }

        [Fact]
        public async Task SetupOtp_UserIsNotExisted_ReturnRedirectToLogout()
        {
            // Arrange
            User user = null;
            var accountController = GetAccountController();
            userService.GetUserByTokenOTPRegisterAsync("token").Returns(user);

            // Act
            var actual = await accountController.SetupOtp("token");

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Auth", viewResult.ControllerName);
            Assert.Equal("Logout", viewResult.ActionName);
        }

        [Fact]
        public async Task SetupOtp_UserIsExisted_ReturnAsExpectedViewResult()
        {
            // Arrange
            var textCode = "TextCode";
            var user = new User { Username = "tim.trinh" };
            var accountController = GetAccountController();
            userService.GetUserByTokenOTPRegisterAsync("token").Returns(user);
            twoFactorAuthService.SetupGoogleAuthInfo(Arg.Any<string>(), Arg.Any<string>()).Returns(textCode);

            // Test
            var actual = await accountController.SetupOtp("token");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<SetupOtpViewModel>(viewResult.Model);
            Assert.Equal("SetupOtp", viewResult.ViewName);
            Assert.Equal(user.Username, viewModel.Username);
            Assert.Equal(textCode, viewModel.TextCode);
        }

        #endregion SetupOtp

        #region SetupOtpAfterReset

        [Fact]
        public async Task SetupOtpAfterReset_EmptyToken_ReturnRedirectToLogout()
        {
            // Arrange
            var accountController = GetAccountController();

            // Act
            var actual = await accountController.SetupOtpAfterReset(string.Empty);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Auth", viewResult.ControllerName);
            Assert.Equal("Logout", viewResult.ActionName);
        }

        [Fact]
        public async Task SetupOtpAfterReset_UserIsNotExisted_ReturnRedirectToLogout()
        {
            // Arrange
            User user = null;
            var accountController = GetAccountController();
            userService.GetUserByTokenOTPRegisterAsync("token").Returns(user);

            // Act
            var actual = await accountController.SetupOtpAfterReset("token");

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Auth", viewResult.ControllerName);
            Assert.Equal("Logout", viewResult.ActionName);
        }

        [Fact]
        public async Task SetupOtpAfterReset_UserIsExisted_ReturnAsExpectedViewResult()
        {
            // Arrange
            var textCode = "TextCode";
            var user = new User
            {
                Username = "tim.trinh",
                FullName = "Tuấn Trần",
                Phone = "0963230397",
                Email = "hunter.tran@nexcel.vn"
            };
            var accountController = GetAccountController();
            userService.GetUserByTokenOTPRegisterAsync(Arg.Any<string>()).Returns(user);
            twoFactorAuthService.SetupGoogleAuthInfo(Arg.Any<string>(), Arg.Any<string>()).Returns(textCode);

            // Act
            var actual = await accountController.SetupOtpAfterReset("token");

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<AccountViewModel>(viewResult.Model);
            Assert.Equal("Index", viewResult.ViewName);
            Assert.Equal(AccountTab.Otp, viewModel.ActivePanel);
            Assert.False(viewModel.IsOTPRegistered);
            Assert.Equal(textCode, viewModel.SetupOtpViewModel.TextCode);
            Assert.Equal(user.Username, viewModel.SetupOtpViewModel.Username);
            Assert.Null(viewModel.ChangePasswordViewModel.OldPassword);
            Assert.Null(viewModel.ChangePasswordViewModel.Password);
            Assert.Null(viewModel.ChangePasswordViewModel.PasswordConfirm);
            Assert.Equal(user.FullName, viewModel.UserProfileViewModel.FullName);
            Assert.Equal(user.Email, viewModel.UserProfileViewModel.Email);
            Assert.Equal(user.Phone, viewModel.UserProfileViewModel.Phone);
        }

        #endregion SetupOtpAfterReset

        #region UpdateProfile

        [Fact]
        public async Task UpdateProfile_ModelStateIsInvalid_ReturnAsExpectedErrors()
        {
            // Arrange
            var model = new UserProfileViewModel();
            var accountController = GetAccountController();
            accountController.ModelState.AddModelError("fullname", "required");

            // Act
            var actual = await accountController.UpdateProfile(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var errors = (IDictionary<string, object>)new RouteValueDictionary(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value));
            Assert.False((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
            Assert.Collection(
                errors,
                kvp =>
                {
                    Assert.Equal("fullname", kvp.Key);
                    Assert.Equal("required", kvp.Value);
                }
            );
        }

        [Fact]
        public async Task UpdateProfile_ModelStateIsValid_ReturnAsExpectedCommandResult()
        {
            // Arrange
            var model = new UserProfileViewModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var accountController = GetAccountController();
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateUserProfileCriteria>()).Returns(expected);

            // Act
            var actual = await accountController.UpdateProfile(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, jsonResult.Value);
        }

        #endregion UpdateProfile

        #region VerifyOTP

        [Fact]
        public void VerifyOTPGet_UserIsAuthenticated_ReturnRedirectToIndex()
        {
            // Arrange
            var accountController = GetAccountController();
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>(), CookieAuthenticationDefaults.AuthenticationScheme))
                }
            };
            accountController.ControllerContext = controllerContext;

            // Act
            var actual = accountController.VerifyOTP();

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", viewResult.ActionName);
            Assert.Collection(
                viewResult.RouteValues,
                kvp =>
                {
                    Assert.Equal("tab", kvp.Key);
                    Assert.Equal(AccountTab.Otp, kvp.Value);
                }
            );
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void VerifyOTPGet_UserIsNotAuthenticatedAndUsernameIsInvalid_ReturnRedirectToLogout(string username)
        {
            // Arrange
            var accountController = GetAccountController();
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()))
                }
            };
            accountController.ControllerContext = controllerContext;
            sessionService.Username = username;

            // Act
            var actual = accountController.VerifyOTP();

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Auth", viewResult.ControllerName);
            Assert.Equal("Logout", viewResult.ActionName);
        }

        [Fact]
        public void VerifyOTPGet_UserIsNotAuthenticatedAndUsernameIsValid_ReturnAsExpectedViewResult()
        {
            // Arrange
            var tuple = new Tuple<string, string>("1234", "1234");
            var accountController = GetAccountController();
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()))
                }
            };
            accountController.ControllerContext = controllerContext;
            sessionService.Username = "hunter.tran";
            rijndaelService.GenerateKey().Returns(tuple);

            // Act
            var actual = accountController.VerifyOTP();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsType<TwoFactorAuthenticatorViewModel>(viewResult.Model);
            Assert.Equal("VerifyOTP", viewResult.ViewName);
            Assert.Null(viewModel.OTPCode);
            Assert.False(viewModel.IsByPassTwoFactor);
        }

        [Fact]
        public async Task VerifyOTPPost_UserIsAuthenticated_ReturnRedirectToIndex()
        {
            // Arrange
            var accountController = GetAccountController();
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>(), CookieAuthenticationDefaults.AuthenticationScheme))
                }
            };
            accountController.ControllerContext = controllerContext;

            // Act
            var actual = await accountController.VerifyOTP(new TwoFactorAuthenticatorViewModel());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", viewResult.ActionName);
            Assert.Collection(
                viewResult.RouteValues,
                kvp =>
                {
                    Assert.Equal("tab", kvp.Key);
                    Assert.Equal(AccountTab.Otp, kvp.Value);
                }
            );
        }

        [Fact]
        public async Task VerifyOTPPost_UserIsNotAuthenticatedAndUserIsNotExisted_ReturnRedirectToLogout()
        {
            // Arrange
            User user = null;
            var accountController = GetAccountController();
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()))
                }
            };
            accountController.ControllerContext = controllerContext;
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(user);

            // Act
            var actual = await accountController.VerifyOTP(new TwoFactorAuthenticatorViewModel());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Auth", viewResult.ControllerName);
            Assert.Equal("Logout", viewResult.ActionName);
        }

        [Fact]
        public async Task VerifyOTPPost_UserIsNotAuthenticatedAndUserIsExistedAndModelStateIsInvalid_ReturnAsExpectedViewResult()
        {
            // Arrange
            var tuple = new Tuple<string, string>("1234", "1234");
            var accountController = GetAccountController();
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()))
                }
            };
            accountController.ControllerContext = controllerContext;
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(new User());
            accountController.ModelState.AddModelError("otpcode", "required");
            sessionService.RijndaelIV = tuple.Item1;
            sessionService.RijndaelKey = tuple.Item2;

            // Act
            var actual = await accountController.VerifyOTP(new TwoFactorAuthenticatorViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<TwoFactorAuthenticatorViewModel>(viewResult.Model);
            Assert.Equal("VerifyOTP", viewResult.ViewName);
            Assert.Equal("The OTP is invalid or expired. Please try another OTP.", viewResult.ViewData["ErrorMessage"]);
            Assert.Empty(viewModelResult.OTPCode);
            Assert.False(viewModelResult.IsByPassTwoFactor);
            // Assert.False(viewModelResult.IsEncrypted);
            Assert.Equal(tuple.Item1, viewModelResult.RijndaelIV);
            Assert.Equal(tuple.Item2, viewModelResult.RijndaelKey);
        }

        [Fact]
        public async Task VerifyOTPPost_UserIsNotAuthenticatedAndUserIsExistedAndModelStateIsValidAndOtpIsInvalid_ReturnAsExpectedViewResult()
        {
            // Arrange
            var tuple = new Tuple<string, string>("1234", "1234");
            var accountController = GetAccountController();
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()))
                }
            };
            accountController.ControllerContext = controllerContext;
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(new User());
            twoFactorAuthService.ValidateOTPCode(Arg.Any<string>(), Arg.Any<string>()).Returns(false);
            sessionService.RijndaelIV = tuple.Item1;
            sessionService.RijndaelKey = tuple.Item2;

            // Act
            var actual = await accountController.VerifyOTP(new TwoFactorAuthenticatorViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModelResult = Assert.IsAssignableFrom<TwoFactorAuthenticatorViewModel>(viewResult.Model);
            Assert.Equal("VerifyOTP", viewResult.ViewName);
            Assert.Equal("The OTP is invalid or expired. Please try another OTP.", viewResult.ViewData["ErrorMessage"]);
            Assert.Empty(viewModelResult.OTPCode);
            Assert.False(viewModelResult.IsByPassTwoFactor);
            Assert.Equal(tuple.Item1, viewModelResult.RijndaelIV);
            Assert.Equal(tuple.Item2, viewModelResult.RijndaelKey);
        }

        [Fact]
        public async Task VerifyOTPPost_UserIsNotAuthenticatedAndUserIsExistedAndModelStateIsValidAndOtpIsValid_ReturnRedirectToIndexHome()
        {
            // Arrange
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            twoFactorAuthService = FakeTwoFactorAuthenticatorService();
            userService = FakeUserService();
            var user = new User { UserId = 1, Username = "hunter.tran", IsSystemAdmin = true };
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
                    User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>())),
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
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(user);
            twoFactorAuthService.ValidateOTPCode(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            otpConfig = FakeOtpConfig();
            rijndaelService = FakeRijndaelService();
            var accountController = Substitute.For<AccountController>(sessionService, baseService, twoFactorAuthService, userService, rijndaelService, otpConfig);
            accountController.ControllerContext = controllerContext;
            accountController.RedirectToAction(Arg.Any<string>(), Arg.Any<string>())
                .Returns(x => new RedirectToActionResult(x.ArgAt<string>(0), x.ArgAt<string>(1), null));

            // Act
            var actual = await accountController.VerifyOTP(new TwoFactorAuthenticatorViewModel { OTPCode = "test" });

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Home", viewResult.ControllerName);
            Assert.Equal("Index", viewResult.ActionName);
        }

        #endregion VerifyOTP

        #region VerifySetupOTP

        [Fact]
        public async Task VerifySetupOTP_UserIsNotExisted_ReturnRedirectToLogout()
        {
            // Arrange
            User user = null;
            var accountController = GetAccountController();
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(user);

            // Act
            var actual = await accountController.VerifySetupOTP(new SetupOtpViewModel());

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Auth", viewResult.ControllerName);
            Assert.Equal("Logout", viewResult.ActionName);
        }

        [Fact]
        public async Task VerifySetupOTP_ModelStateIsInvalid_ReturnForbidResult()
        {
            // Arrange
            var accountController = GetAccountController();
            accountController.ModelState.AddModelError("otp", "required");
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(new User());
            twoFactorAuthService.ValidateOTPCode(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            // Act
            var actual = await accountController.VerifySetupOTP(new SetupOtpViewModel());

            // Assert
            Assert.IsType<ForbidResult>(actual);
        }

        [Fact]
        public async Task VerifySetupOTP_OtpIsInvalid_ReturnForbidResult()
        {
            // Arrange
            var accountController = GetAccountController();
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(new User());
            twoFactorAuthService.ValidateOTPCode(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            // Act
            var actual = await accountController.VerifySetupOTP(new SetupOtpViewModel());

            // Assert
            Assert.IsType<ForbidResult>(actual);
        }

        [Fact]
        public async Task VerifySetupOTP_ModelStateIsValidAndOtpIsValid_ReturnOkResult()
        {
            // Arrange
            var accountController = GetAccountController();
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(new User());
            twoFactorAuthService.ValidateOTPCode(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            // Act
            var actual = await accountController.VerifySetupOTP(new SetupOtpViewModel());

            // Assert
            Assert.IsType<OkResult>(actual);
        }

        #endregion VerifySetupOTP

        #region Helpers

        private static IBaseService FakeBaseService()
        {
            return Substitute.For<IBaseService>();
        }

        private static ISessionService FakeSessionService()
        {
            return Substitute.For<ISessionService>();
        }

        private static ITwoFactorAuthenticatorService FakeTwoFactorAuthenticatorService()
        {
            return Substitute.For<ITwoFactorAuthenticatorService>();
        }

        private static IUserService FakeUserService()
        {
            return Substitute.For<IUserService>();
        }

        private IOptionsSnapshot<OtpConfig> FakeOtpConfig()
        {
            var config = new OtpConfig
            {
                AppName = "HRP",
                OtpExpireTime = 0
            };

            var testOtpConfig = Substitute.For<IOptionsSnapshot<OtpConfig>>();
            testOtpConfig.Value.Returns(config);

            return testOtpConfig;
        }

        private static IRijndaelService FakeRijndaelService()
        {
            var service = Substitute.For<IRijndaelService>();

            return service;
        }

        private AccountController GetAccountController()
        {
            sessionService = FakeSessionService();
            baseService = FakeBaseService();
            twoFactorAuthService = FakeTwoFactorAuthenticatorService();
            userService = FakeUserService();
            otpConfig = FakeOtpConfig();
            rijndaelService = FakeRijndaelService();

            return new AccountController(sessionService, baseService, twoFactorAuthService, userService, rijndaelService, otpConfig);
        }

        #endregion Helpers
    }
}