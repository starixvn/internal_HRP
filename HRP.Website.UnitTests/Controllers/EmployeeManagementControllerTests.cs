namespace HRP.Website.UnitTests.Controllers
{
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using HRP.Criteria.Employee;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class EmployeeManagementControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IHostingEnvironment hostingEnvironment;
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

        [Fact]
        public async Task CheckEmployeeCodeExisted_WhenCalled_ReturnAsExpected()
        {
            //Arrange
            var expected = true;
            employeeManagementController = GetEmployeeManagementController();
            baseService.GetDataEntityAsync<bool>(Arg.Any<CheckEmpCodeExistedCriteria>()).Returns(expected);

            //Act
            var actual = await employeeManagementController.CheckEmployeeCodeExisted("DAR", "1234");

            //Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.Equal(expected, (bool)jsonResult.Value);
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

        private IHostingEnvironment FakeHostingEnvironment()
        {
            var environment = Substitute.For<IHostingEnvironment>();

            return environment;
        }

        private EmployeeManagementController GetEmployeeManagementController()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            hostingEnvironment = FakeHostingEnvironment();
            var controller = new EmployeeManagementController(sessionService, baseService, hostingEnvironment);

            return controller;
        }

        #endregion Helpers
    }
}