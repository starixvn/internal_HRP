namespace HRP.Website.UnitTests.Controllers
{
    using HRP.Core.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class EmployeeManagementControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private EmployeeManagementController employeeManagementController;

        [Fact]
        public void Index_WhenCalled_ReturnsViewResult()
        {
            //Arrange
            employeeManagementController = GetEmployeeManagementController();

            //Act
            var actual = employeeManagementController.Index();

            //Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
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

        private EmployeeManagementController GetEmployeeManagementController()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            var controller = new EmployeeManagementController(sessionService, baseService);

            return controller;
        }

        #endregion Helpers
    }
}