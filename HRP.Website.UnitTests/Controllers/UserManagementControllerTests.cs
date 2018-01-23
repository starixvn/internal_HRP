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

        #region AssignPermission

        [Fact]
        public async Task AssignPermission_WhenCalled_ReturnExpectedViewResult()
        {
            // Arrange
            IEnumerable<Company> companies = new List<Company>();
            IEnumerable<Models.UserPermisson.FunctionModel> functions = new List<Models.UserPermisson.FunctionModel>();
            IEnumerable<UserPermissionModel> permissions = new List<UserPermissionModel>();
            var user = new UserModel { Username = "tim", IsSystemAdmin = false };
            var userController = GetUserManagementController();
            functionService.GetTreeFunctions(Arg.Any<bool?>()).Returns(functions);
            baseService.GetDataAsync<Company>(Arg.Any<GetAllCompanyCriteria>()).Returns(companies);
            baseService.GetDataAsync<UserPermissionModel>(Arg.Any<GetFunctionByUserIdCriteria>()).Returns(permissions);
            baseService.GetDataEntityAsync<UserModel>(Arg.Any<GetUserByIdCriteria>()).Returns(user);
            sessionService.HasPermissionOnFunction(Arg.Any<string>(), Arg.Any<Policy>()).Returns(true);

            // Act
            var actual = await userController.AssignPermission(1);

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
            var userController = GetUserManagementController();
            baseService.ExecuteNonQueryAsync(Arg.Any<AssignFunctionToUserCriteria>()).Returns(Task.FromResult(expected));

            // Act
            var actual = await userController.AssignPermission(permissions);

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
            var userController = GetUserManagementController();

            // Act
            var actual = userController.CreateUser();

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
            var userController = GetUserManagementController();
            userController.ModelState.AddModelError("", "Error");

            // Act
            var actual = userController.CreateUser(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotEmpty(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value) as IEnumerable);
        }

        [Fact]
        public void CreateUser_ValidModel_ReturnSucessfully()
        {
            // Arrange
            var model = new CreateUserViewModel { Password = "Admin1234" };
            var userController = GetUserManagementController();
            baseService.ExecuteNonQuery(Arg.Any<CreateUserCriteria>()).Returns(new CommandResult { Code = 0, Message = string.Empty });
            userController.Url = Substitute.For<IUrlHelper>();
            userController.Url.Action().ReturnsForAnyArgs("/UserManagement/AssignPermissions");

            // Act
            var actual = userController.CreateUser(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public void EditUser_InvalidModel_ReturnErrors()
        {
            // Arrange
            var model = new EditUserViewModel();
            var userController = GetUserManagementController();
            userController.ModelState.AddModelError("", "Error");

            // Act
            var actual = userController.EditUser(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotEmpty(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value) as IEnumerable);
        }

        [Fact]
        public void EditUser_ValidModel_ReturnSucessfully()
        {
            // Arrange
            var model = new EditUserViewModel();
            var userController = GetUserManagementController();
            baseService.ExecuteNonQuery(Arg.Any<EditUserCriteria>()).Returns(new CommandResult { Code = 0, Message = string.Empty });

            // Act
            var actual = userController.EditUser(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("IsSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public void EditUser_InvalidId_RetdirectToIndexPage()
        {
            // Arrange
            var userController = GetUserManagementController();

            // Act
            var actual = userController.EditUser(0);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("Index", viewResult.ActionName);
        }

        [Fact]
        public void EditUser_ValidId_ReturnExpectedView()
        {
            // Arrange
            var id = 1;
            var userController = GetUserManagementController();
            baseService.GetDataEntity<EditUserViewModel>(Arg.Any<GetUserByIdCriteria>()).Returns(new EditUserViewModel { UserId = id, IsSystemAdmin = false });
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            // Act
            var actual = userController.EditUser(id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<EditUserViewModel>(viewResult.Model);
        }

        [Fact]
        public void Index_WhenCalled_ReturnExpectedView()
        {
            // Arrange
            var userController = GetUserManagementController();
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            // Act
            var actual = userController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            Assert.IsAssignableFrom<UserManagementViewModel>(viewResult.Model);
        }

        [Fact]
        public void GetUsers_WhenCalled_ReturnExpectedView()
        {
            // Arrange
            var userController = GetUserManagementController();
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            // Act
            var actual = userController.GetUsers(true);

            // Assert
            Assert.IsType<ViewComponentResult>(actual);
        }

        [Fact]
        public void CheckUsernameExisted_ExistedUserName_ReturnFalse()
        {
            // Arrange
            var username = "admin";
            var userController = GetUserManagementController();
            baseService.GetData<int>(Arg.Any<CheckUsernameExistedCriteria>()).Returns(new List<int> { 1 });

            // Act
            var actual = userController.CheckUsernameExisted(username);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.False((bool)jsonResult.Value);
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

        private static IFunctionService FakeFunctionService()
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