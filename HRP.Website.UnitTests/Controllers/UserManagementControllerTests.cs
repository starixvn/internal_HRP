namespace HRP.Website.UnitTests.Controllers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
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
            IEnumerable<Models.UserPermisson.FunctionModel> functions = new List<Models.UserPermisson.FunctionModel>();
            IEnumerable<UserPermissionModel> permissions = new List<UserPermissionModel>();
            var user = new UserModel { Username = "tim", IsSystemAdmin = false };
            userManagementController = GetUserManagementController();
            functionService.GetTreeFunctions(Arg.Any<bool?>()).Returns(functions);
            baseService.GetDataAsync<Company>(Arg.Any<GetAllCompanyCriteria>()).Returns(companies);
            baseService.GetDataAsync<UserPermissionModel>(Arg.Any<GetFunctionByUserIdCriteria>()).Returns(permissions);
            baseService.GetDataEntityAsync<UserModel>(Arg.Any<GetUserByIdCriteria>()).Returns(user);
            sessionService.HasPermissionOnFunction(Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);

            // Act
            var actual = await userManagementController.AssignPermission(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<AssignPermissonViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.True(viewModel.IsHost);
            Assert.Equal(user.Username, viewModel.Username);
            Assert.Equal(companies, viewModel.Companies);
            Assert.Equal(functions, viewModel.Functions);
            Assert.Equal(permissions, viewModel.UserPermissions);
        }

        [Fact]
        public async Task AssignPermissionPost_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var expected = new CommandResult
            {
                Code = 0
            };
            var permissions = new List<UserPermissionModel>
            {
                new UserPermissionModel { UserId = 1, FunctionId = 1, CompanyId = 1, IsHost = true, IsRead = true }
            };
            userManagementController = GetUserManagementController();
            baseService.ExecuteNonQueryAsync(Arg.Any<AssignFunctionToUserCriteria>()).Returns(Task.FromResult(expected));

            // Act
            var actual = await userManagementController.AssignPermission(permissions);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            var commandResult = Assert.IsAssignableFrom<CommandResult>(jsonResult.Value);
            Assert.True(commandResult.IsSuccessful);
            Assert.Equal(expected.Code, commandResult.Code);
            Assert.Equal(expected.Message, commandResult.Message);
        }

        #endregion AssignPermission

        [Fact]
        public void CreateUser_WhenCalled_ReturnExpectedView()
        {
            // Arrange
            userManagementController = GetUserManagementController();

            // Act
            var actual = userManagementController.CreateUser();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.Null(viewResult.ViewName);
            Assert.IsAssignableFrom<CreateUserViewModel>(viewResult.Model);
        }

        [Fact]
        public void CreateUser_InvalidModel_ReturnErrors()
        {
            // Arrange
            var model = new CreateUserViewModel();
            userManagementController = GetUserManagementController();
            userManagementController.ModelState.AddModelError("", "Error");

            // Act
            var actual = userManagementController.CreateUser(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotEmpty(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value) as IEnumerable);
        }

        [Fact]
        public void CreateUser_ValidModel_ReturnSucessfully()
        {
            // Arrange
            var model = new CreateUserViewModel { Password = "Admin1234" };
            userManagementController = GetUserManagementController();
            baseService.ExecuteNonQuery(Arg.Any<CreateUserCriteria>()).Returns(new CommandResult { Code = 0, Message = string.Empty });
            userManagementController.Url = Substitute.For<IUrlHelper>();
            userManagementController.Url.Action().ReturnsForAnyArgs("/UserManagement/AssignPermissions");

            // Act
            var actual = userManagementController.CreateUser(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public void EditUser_InvalidModel_ReturnErrors()
        {
            // Arrange
            var model = new EditUserViewModel();
            userManagementController = GetUserManagementController();
            userManagementController.ModelState.AddModelError("", "Error");

            // Act
            var actual = userManagementController.EditUser(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotEmpty(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value) as IEnumerable);
        }

        [Fact]
        public void EditUser_ValidModel_ReturnSucessfully()
        {
            // Arrange
            var model = new EditUserViewModel();
            userManagementController = GetUserManagementController();
            baseService.ExecuteNonQuery(Arg.Any<EditUserCriteria>()).Returns(new CommandResult { Code = 0, Message = string.Empty });

            // Act
            var actual = userManagementController.EditUser(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("IsSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public void EditUser_InvalidId_RetdirectToIndexPage()
        {
            // Arrange
            userManagementController = GetUserManagementController();

            // Act
            var actual = userManagementController.EditUser(0);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public void EditUser_ValidId_ReturnExpectedView()
        {
            // Arrange
            var id = 1;
            userManagementController = GetUserManagementController();
            baseService.GetDataEntity<EditUserViewModel>(Arg.Any<GetUserByIdCriteria>()).Returns(new EditUserViewModel { UserId = id, IsSystemAdmin = false });
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            // Act
            var actual = userManagementController.EditUser(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<EditUserViewModel>(viewResult.Model);
        }

        [Fact]
        public void Index_WhenCalled_ReturnExpectedView()
        {
            // Arrange
            userManagementController = GetUserManagementController();
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            // Act
            var actual = userManagementController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<UserManagementViewModel>(viewResult.Model);
        }

        [Fact]
        public void GetUsers_WhenCalled_ReturnExpectedView()
        {
            // Arrange
            userManagementController = GetUserManagementController();
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            // Act
            var actual = userManagementController.GetUsers(true);

            // Assert
            Assert.IsType<ViewComponentResult>(actual);
        }

        [Fact]
        public void CheckUsernameExisted_ExistedUserName_ReturnFalse()
        {
            // Arrange
            var username = "admin";
            userManagementController = GetUserManagementController();
            baseService.GetData<int>(Arg.Any<CheckUsernameExistedCriteria>()).Returns(new List<int> { 1 });

            // Act
            var actual = userManagementController.CheckUsernameExisted(username);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value);
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