namespace HRP.Website.UnitTests.Controllers
{
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Xunit;

    public class ErrorControllerTests
    {
        private ErrorController errorController;

        [Fact]
        public void PageNotFound_WhenCalled_ReturnsViewResult()
        {
            //Arrange
            errorController = GetErrorController();

            //Act
            var actual = errorController.PageNotFound();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void AccessDenied_WhenCalled_ReturnsViewResult()
        {
            //Arrange
            errorController = GetErrorController();

            //Act
            var actual = errorController.AccessDenied();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        [Fact]
        public void InternalError_WhenCalled_ReturnsViewResult()
        {
            //Arrange
            errorController = GetErrorController();

            //Act
            var actual = errorController.InternalError();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
        }

        #region Helpers

        private ErrorController GetErrorController()
        {
            var controller = new ErrorController();

            return controller;
        }

        #endregion Helpers
    }
}