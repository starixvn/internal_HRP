namespace HRP.Website.UnitTests.Controllers
{
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
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

        private static HomeController GetHomeController()
        {
            var controller = new HomeController();

            return controller;
        }

        #endregion Helpers
    }
}