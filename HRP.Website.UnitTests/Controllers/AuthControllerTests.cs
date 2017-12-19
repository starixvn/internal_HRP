namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Account;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class AuthControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IUserService userService;
        private AuthController _authController;

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
        public void Login_None_ReturnViewResult()
        {
            _authController = GetAuthController();

            var actual = _authController.Login();

            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void ConfirmationEmail_None_ReturnViewResult()
        {
            _authController = GetAuthController();

            var actual = _authController.ConfirmationEmailSent();

            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void ForgotPassword_None_ReturnViewResult()
        {
            _authController = GetAuthController();

            var actual = _authController.ForgotPassword();

            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void Login_ValidModel_ReturnSuccessfully()
        {
            _authController = GetAuthController();

            var model = new LoginViewModel
            {
                Username = "admin",
                Password = "r2RzX0EqeQCSjpXCp/GMnA=="
            };
            var actual = _authController.Login(model);
            Assert.IsType<ViewResult>(actual);
        }

        [Fact]
        public void ResetPassword_NullOrEmptyToken_ReturnNotFound()
        {
            _authController = GetAuthController();
            var actual = _authController.ResetPassword(string.Empty);
            Assert.IsType<NotFoundResult>(actual);
        }

        [Fact]
        public void ResetPassword_InvalidModel_ReturnViewResult()
        {
            _authController = GetAuthController();
            var model = new ResetPasswordViewModel();
            _authController.ModelState.AddModelError("", "Error");
            var actual = _authController.ResetPassword("test", model);
            Assert.IsType<ViewResult>(actual);
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

            service.GetUserByUsername(Arg.Any<string>()).Returns(user);

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