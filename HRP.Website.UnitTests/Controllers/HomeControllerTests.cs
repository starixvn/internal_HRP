namespace HRP.Website.UnitTests.Controllers
{
    using HRP.Core.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class HomeControllerTests
    {
        [Fact]
        public void Index_WhenCalled_ReturnsViewResult()
        {
            //Arrange
            var homeController = GetHomeController();

            //Act
            var actual = homeController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        #region Helpers

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private static HomeController GetHomeController()
        {
            var sessionService = FakeSessionService();
            var baseService = FakeBaseService();
            var controller = new HomeController(sessionService, baseService);

            return controller;
        }

        #endregion Helpers
    }
}