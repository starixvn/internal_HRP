namespace HRP.Website.UnitTests.Controllers
{
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Xunit;

    public class EmployeeManagementControllerTests
    {
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

        private EmployeeManagementController GetEmployeeManagementController()
        {
            var controller = new EmployeeManagementController();

            return controller;
        }

        #endregion Helpers
    }
}