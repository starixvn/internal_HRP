namespace HRP.Website.UnitTests.Controllers
{
    using HRP.Core.Criteria;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Account;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using System;
    using System.Threading.Tasks;
    using Xunit;

    public class AuthControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IUserService userService;
        private AuthController authController;

        private User user = new User
        {
            UserId = 16,
            Username = "oliver",
            FullName = "Hải",
            Phone = "0909120393",
            Email = "oliver.dang@nexcel.vn",
            Password = "$2a$10$lTZ4z7TmyQhLYoBFLS6p9uqqCGQAmxOkaPREcqapnUNsmRPzKdvUy",
            IsActive = true,
            LastLoginDate = DateTime.Parse("2017-12-13 13:51:59.087"),
            IsOTPRegistered = true,
            SecretKey = "4b7be5aea6fd",
            CreatedBy = 1,
            CreatedDate = DateTime.Parse("2017-12-11 09:04:56.543"),
            IsSystemAdmin = false,
        };

        [Fact]
        public void Login_ReturnExpectedViewResult()
        {
            authController = GetAuthController();

            var actual = authController.Login();

            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void ConfirmationEmail_ReturnExpectedViewResult()
        {
            authController = GetAuthController();

            var actual = authController.ConfirmationEmailSent();

            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void ResetPasswordSuccessfully_ReturnExpectedViewResult()
        {
            authController = GetAuthController();
            var actual = authController.ResetPasswordSuccessfully();
            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void ForgotPassword_ValidUser_RedirectSuccessful()
        {
            authController = GetAuthController();
            userService.GetUserByUsername(Arg.Any<string>()).Returns(user);
            var actual = authController.ForgotPassword("test");
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("ConfirmationEmailSent", viewResult.ActionName);
        }

        [Fact]
        public void Login_ValidModel_ReturnSuccessfully()
        {
            authController = GetAuthController();

            var model = new LoginViewModel
            {
                Username = "admin",
                Password = "r2RzX0EqeQCSjpXCp/GMnA=="
            };
            var actual = authController.Login(model);
            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void Login_ValidUser_NotRegisterOTP_RedirectNotificationView()
        {
            authController = GetAuthController();

            var model = new LoginViewModel
            {
                Username = "admin",
                Password = "r2RzX0EqeQCSjpXCp/GMnA=="
            };
            userService.GetUserByUsername(Arg.Any<string>()).Returns(user);
            userService.IsCorrectPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            user.IsOTPRegistered = false;
            var actual = authController.Login(model);
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("RegisterOTPNotification", viewResult.ActionName);
        }

        [Fact]
        public void Login_ValidUser_RegisteredOTP_RedirectToVerifyOTPView()
        {
            authController = GetAuthController();

            var model = new LoginViewModel
            {
                Username = "admin",
                Password = "r2RzX0EqeQCSjpXCp/GMnA=="
            };
            userService.GetUserByUsername(Arg.Any<string>()).Returns(user);
            userService.IsCorrectPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            user.IsOTPRegistered = true;
            var actual = authController.Login(model);
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Account", viewResult.ControllerName);
            Assert.Equal("VerifyOTP", viewResult.ActionName);
        }

        [Fact]
        public async Task Logout_Success_RedirectToLogin()
        {
            authController = GetAuthController();
            baseService.ExecuteNonQueryAsync(Arg.Any<LogOutUserCriteria>()).Returns(new CommandResult { Code = 0 });
            var actual = await authController.Logout();
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Login", viewResult.ActionName);
        }

        [Fact]
        public void ResetPassword_InvalidUser_RedirectTo404()
        {
            authController = GetAuthController();
            baseService.GetDataEntity<User>(Arg.Any<GetUserByTokenResetPasswordCriteria>()).Returns(new User());
            var actual = authController.ResetPassword("test");
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("error", viewResult.ControllerName);
            Assert.Equal("404", viewResult.ActionName);
        }

        [Fact]
        public void ResetPassword_InvalidModel_ReturnViewResult()
        {
            authController = GetAuthController();
            var model = new ResetPasswordViewModel();
            authController.ModelState.AddModelError("", "Error");
            var actual = authController.ResetPassword("test", model);
            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void ResetPassword_ValidModel_RedirectToAction()
        {
            authController = GetAuthController();
            baseService.GetDataEntity<User>(Arg.Any<GetUserByTokenResetPasswordCriteria>()).Returns(user);
            var model = new ResetPasswordViewModel { Password = "1234aa" };
            baseService.ExecuteNonQuery(Arg.Any<UpdateUserPasswordCriteria>()).Returns(new CommandResult { Code = 0, Message = string.Empty });
            var actual = authController.ResetPassword("abc", model);
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("ResetPasswordSuccessfully", viewResult.ActionName);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;
            return service;
        }

        private IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private IUserService FakeUserService()
        {
            var service = Substitute.For<IUserService>();

            return service;
        }

        private AuthController GetAuthController()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            userService = FakeUserService();
            var controller = new AuthController(userService, sessionService, baseService);

            return controller;
        }

        #endregion Helpers
    }
}