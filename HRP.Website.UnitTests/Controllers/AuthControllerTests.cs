namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Threading.Tasks;
    using HRP.Core.Criteria;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Account;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class AuthControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IUserService userService;

        private readonly User user = new User
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

        #region Login

        [Fact]
        public void LoginGet_WhenCalled_ExpectedClearSession()
        {
            // Arrange
            var authController = GetAuthController();

            // Act
            authController.Login();

            // Assert
            sessionService.Received().ClearAll();
        }

        [Fact]
        public void LoginGet_WhenCalled_ReturnExpectedViewResult()
        {
            // Arrange
            var authController = GetAuthController();

            // Act
            var actual = authController.Login();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<LoginViewModel>(viewResult.ViewData.Model);
            Assert.Null(viewResult.ViewName);
            Assert.Null(viewModel.Username);
            Assert.Null(viewModel.Password);
        }

        [Fact]
        public async Task LoginPost_ValidModel_ReturnSuccessfully()
        {
            //Arrange
            var authController = GetAuthController();
            var model = new LoginViewModel
            {
                Username = "admin",
                Password = "r2RzX0EqeQCSjpXCp/GMnA=="
            };

            //Act
            var actual = await authController.Login(model);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Login_CorrectUserAndNotRegisterOTP_RedirectNotificationView()
        {
            //Arrange
            var authController = GetAuthController();
            var model = new LoginViewModel
            {
                Username = "admin",
                Password = "r2RzX0EqeQCSjpXCp/GMnA=="
            };
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(user);
            userService.IsCorrectPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            user.IsOTPRegistered = false;

            //Act
            var actual = await authController.Login(model);

            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("RegisterOTPNotification", viewResult.ActionName);
        }

        [Fact]
        public async Task Login_CorrectUserAndRegisteredOTP_RedirectToVerifyOTPView()
        {
            //Arrange
            var authController = GetAuthController();
            var model = new LoginViewModel
            {
                Username = "admin",
                Password = "r2RzX0EqeQCSjpXCp/GMnA=="
            };
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(user);
            userService.IsCorrectPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(true);
            user.IsOTPRegistered = true;

            //Act
            var actual = await authController.Login(model);

            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Account", viewResult.ControllerName);
            Assert.Equal("VerifyOTP", viewResult.ActionName);
        }

        [Fact]
        public async Task Login_IncorrectUser_ReturnViewResultWithModel()
        {
            //Arrange
            var authController = GetAuthController();
            var model = new LoginViewModel
            {
                Username = "admin",
                Password = "r2RzX0EqeQCSjpXCp/GMnA=="
            };
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(user);
            userService.IsCorrectPassword(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            //Act
            var actual = await authController.Login(model);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<LoginViewModel>(viewResult.ViewData.Model);
            Assert.Equal(1, viewResult.ViewData.ModelState.ErrorCount);
        }

        #endregion Login

        #region ForgotPassword

        [Fact]
        public void ForgotPasswordGet_WhenCalled_ReturnExpectedViewResult()
        {
            // Arrange
            var authController = GetAuthController();

            // Act
            var actual = authController.ForgotPassword();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
            Assert.Null(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task ForgotPassword_CorrectUser_RedirectSuccessful()
        {
            //Arrange
            var authController = GetAuthController();
            userService.GetUserByUsernameAsync(Arg.Any<string>()).Returns(user);

            //Act
            var actual = await authController.ForgotPassword("test");

            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("ConfirmationEmailSent", viewResult.ActionName);
        }

        [Fact]
        public async Task ForgotPassword_IncorrectUsername_ReturnViewResult()
        {
            //Arrange
            var authController = GetAuthController();

            //Act
            var actual = await authController.ForgotPassword("test");

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Equal(1, viewResult.ViewData.ModelState.ErrorCount);
        }

        #endregion ForgotPassword

        [Fact]
        public void ConfirmationEmailSent_ReturnExpectedViewResult()
        {
            // Arrange
            var authController = GetAuthController();

            // Act
            var actual = authController.ConfirmationEmailSent();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
            Assert.Null(viewResult.ViewData.Model);
        }

        [Fact]
        public async Task ResetPassword_ValidModelAndUpdateSuccessful_RedirectToAction()
        {
            //Arrange
            var authController = GetAuthController();
            baseService.GetDataEntityAsync<User>(Arg.Any<GetUserByTokenResetPasswordCriteria>()).Returns(user);
            var model = new ResetPasswordViewModel { Password = "1234aa", PasswordConfirm = "1234aa" };
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateUserPasswordCriteria>()).Returns(new CommandResult { Code = 0, Message = string.Empty });

            //Act
            var actual = await authController.ResetPassword("abc", model);

            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("ResetPasswordSuccessfully", viewResult.ActionName);
        }

        [Fact]
        public async Task ResetPassword_ValidModelAndUpdateFail_ReturnViewResult()
        {
            //Arrange
            var authController = GetAuthController();
            baseService.GetDataEntityAsync<User>(Arg.Any<GetUserByTokenResetPasswordCriteria>()).Returns(new User());
            var model = new ResetPasswordViewModel { Password = "1234aa" };
            baseService.ExecuteNonQueryAsync(Arg.Any<UpdateUserPasswordCriteria>()).Returns(new CommandResult { Code = -1, Message = string.Empty });

            //Act
            var actual = await authController.ResetPassword("abc", model);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Equal(1, viewResult.ViewData.ModelState.ErrorCount);
        }

        [Fact]
        public async Task ResetPassword_IncorrectUser_RedirectToPageNotFound()
        {
            //Arrange
            var authController = GetAuthController();
            baseService.GetDataEntityAsync<User>(Arg.Any<GetUserByTokenResetPasswordCriteria>()).Returns(new User());

            //Act
            var actual = await authController.ResetPassword("test");

            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task ResetPassword_CorrectUser_ReturnViewResult()
        {
            //Arrange
            var authController = GetAuthController();
            baseService.GetDataEntityAsync<User>(Arg.Any<GetUserByTokenResetPasswordCriteria>()).Returns(user);

            //Act
            var actual = await authController.ResetPassword("test");

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task ResetPassword_InvalidModel_ReturnViewResult()
        {
            //Arrange
            var authController = GetAuthController();
            var model = new ResetPasswordViewModel();
            authController.ModelState.AddModelError("", "Error");

            //Act
            var actual = await authController.ResetPassword("test", model);

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<ResetPasswordViewModel>(viewResult.ViewData.Model);
        }

        [Fact]
        public void ResetPasswordSuccessfully_NoModel_ReturnExpectedViewResult()
        {
            //Arrange
            var authController = GetAuthController();

            //Act
            var actual = authController.ResetPasswordSuccessfully();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public async Task Logout_Successful_RedirectToLogin()
        {
            //Arrange
            var authController = ArrangeForSignOutTestFunction();

            //Act
            var actual = await authController.Logout();

            //Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Login", viewResult.ActionName);
        }

        #region Helpers

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private static IUserService FakeUserService()
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

        private AuthController ArrangeForSignOutTestFunction()
        {
            var authService = Substitute.For<IAuthenticationService>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IAuthenticationService)).Returns(authService);
            var controllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    RequestServices = serviceProvider
                }
            };

            var authController = GetAuthController();
            authController.ControllerContext = controllerContext;
            authController.ControllerContext.HttpContext.Connection.RemoteIpAddress = System.Net.IPAddress.Any;
            authController.ControllerContext.HttpContext.Request.Headers["User-Agent"] = "12345";
            authController.Url = Substitute.For<IUrlHelper>();
            baseService.ExecuteNonQueryAsync(Arg.Any<LogOutUserCriteria>()).Returns(new CommandResult { Code = 0 });

            return authController;
        }

        #endregion Helpers
    }
}