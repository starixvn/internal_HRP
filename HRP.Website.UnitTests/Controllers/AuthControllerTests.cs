namespace HRP.Website.UnitTests.Controllers
{
    using HRP.Core.Services;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class AuthControllerTests
    {
        private readonly AuthController _authController;

        public AuthControllerTests()
        {
            var baseService = Substitute.For<IBaseService>();
            var userService = Substitute.For<IUserService>();
            var sessionService = Substitute.For<ISessionService>();

            _authController = new AuthController(
                userService,
                sessionService,
                baseService);
        }

        [Fact]
        public void Login_None_ReturnViewResult()
        {
            var actual = _authController.Login();

            Assert.IsType<ViewResult>(actual);
        }
    }
}