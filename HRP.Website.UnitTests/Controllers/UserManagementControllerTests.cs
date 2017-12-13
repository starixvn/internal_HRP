namespace HRP.Website.UnitTests.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.UserPermisson;
    using HRP.Services;
    using HRP.Website.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class UserManagementControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IFunctionService functionService;
        private UserManagementController userManagementController;

        #region AssignPermission

        [Fact]
        public async Task AssignPermission_WhenCalled_ReturnExpectedViewResult()
        {
            // Arrange
            IEnumerable<Company> companies = new List<Company>();
            IEnumerable<HRP.Models.UserPermisson.FunctionModel> functions = new List<HRP.Models.UserPermisson.FunctionModel>();
            IEnumerable<UserPermissionModel> permissions = new List<UserPermissionModel>();
            var user = new UserModel { Username = "tim", IsSystemAdmin = false };
            userManagementController = GetUserManagementController();
            functionService.GetTreeFunctions(Arg.Any<bool?>()).Returns(functions);
            baseService.GetDataAsync<Company>(Arg.Any<GetAllCompanyCriteria>()).Returns(companies);
            baseService.GetDataAsync<UserPermissionModel>(Arg.Any<GetFunctionByUserIdCriteria>()).Returns(permissions);
            baseService.GetDataEntityAsync<UserModel>(Arg.Any<GetUserByIdCriteria>()).Returns(user);
            sessionService.HasPermissionOnFunction(Arg.Any<string>(), Arg.Any<Core.Enumerations.Policy>()).Returns(true);

            // Act
            var actual = await userManagementController.AssignPermission(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<AssignPermissonViewModel>(viewResult.Model);
            Assert.True(viewModel.IsHost);
            Assert.Equal(user.Username, viewModel.Username);
            Assert.Equal(companies, viewModel.Companies);
            Assert.Equal(functions, viewModel.Functions);
            Assert.Equal(permissions, viewModel.UserPermissions);
        }

        [Fact]
        public async Task AssignPermissionPost_WhenCalled_ReturnExpectedResult()
        {
            // Arrange
            var expected = new CommandResult();
            var permissions = string.Empty;
            userManagementController = GetUserManagementController();
            baseService.ExecuteNonQueryAsync(Arg.Any<AssignFunctionToUserCriteria>()).Returns(Task.FromResult(expected));

            // Act
            var actual = await userManagementController.AssignPermission(permissions);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var value = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.False(value.IsSuccessful);
        }

        #endregion AssignPermission

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

        private IFunctionService FakeFunctionService()
        {
            var service = Substitute.For<IFunctionService>();

            return service;
        }

        private UserManagementController GetUserManagementController()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            functionService = FakeFunctionService();
            var controller = new UserManagementController(sessionService, baseService, functionService);

            return controller;
        }

        #endregion Helpers
    }
}