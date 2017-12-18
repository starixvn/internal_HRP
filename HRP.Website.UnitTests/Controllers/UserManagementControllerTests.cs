namespace HRP.Website.UnitTests.Controllers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.UserManagement;
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

        [Fact]
        public void CreateUser_WhenCalled_ReturnExpectedView()
        {
            userManagementController = GetUserManagementController();
            var actual = userManagementController.CreateUser();
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<CreateUserViewModel>(viewResult.Model);
        }

        [Fact]
        public void CreateUser_InvalidModel_ReturnErrors()
        {
            var model = new CreateUserViewModel();
            userManagementController = GetUserManagementController();

            userManagementController.ModelState.AddModelError("", "Error");
            var actual = userManagementController.CreateUser(model);

            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotEmpty(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value) as IEnumerable);
        }

        [Fact]
        public void CreateUser_ValidModel_ReturnSucessfully()
        {
            var model = new CreateUserViewModel { Password = "Admin1234" };
            userManagementController = GetUserManagementController();

            baseService.ExecuteNonQuery(Arg.Any<CreateUserCriteria>()).Returns(new CommandResult { Code = 0, Message = string.Empty });
            userManagementController.Url = Substitute.For<IUrlHelper>();
            userManagementController.Url.Action().ReturnsForAnyArgs("/UserManagement/AssignPermissions");

            var actual = userManagementController.CreateUser(model);

            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public void EditUser_InvalidModel_ReturnErrors()
        {
            var model = new EditUserViewModel();
            userManagementController = GetUserManagementController();

            userManagementController.ModelState.AddModelError("", "Error");
            var actual = userManagementController.EditUser(model);

            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotEmpty(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value) as IEnumerable);
        }

        [Fact]
        public void EditUser_ValidModel_ReturnSucessfully()
        {
            var model = new EditUserViewModel();
            userManagementController = GetUserManagementController();

            baseService.ExecuteNonQuery(Arg.Any<EditUserCriteria>()).Returns(new CommandResult { Code = 0, Message = string.Empty });
            var actual = userManagementController.EditUser(model);

            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("IsSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public void EditUser_InvalidId_RetdirectToIndexPage()
        {
            userManagementController = GetUserManagementController();

            var actual = userManagementController.EditUser(0);
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public void EditUser_ValidId_ReturnExpectedView()
        {
            var id = 1;
            userManagementController = GetUserManagementController();
            baseService.GetDataEntity<EditUserViewModel>(Arg.Any<GetUserByIdCriteria>()).Returns(new EditUserViewModel { UserId = id, IsSystemAdmin = false });

            var actual = userManagementController.EditUser(id);
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<EditUserViewModel>(viewResult.Model);
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