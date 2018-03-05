namespace HRP.Website.UnitTests.Controllers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.UserManagement;
    using Models.UserPermisson;
    using NSubstitute;
    using Services;
    using Website.Controllers;
    using Xunit;

    public class UserManagementControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private IFunctionService functionService;

        [Fact]
        public async Task Index_WhenCalled_ReturnExpectedViewResult()
        {
            // Arrange
            var users = new List<UserModel>
            {
                new UserModel { UserId = 1, Username = "tim.trinh", FullName = "thang", Email = "tim.trinh@gmail.com", IsActive = true },
                new UserModel { UserId = 2, Username = "hunter.tran", FullName = "tuan", Email = "hunter.tran@gmail.com", IsActive = false },
                new UserModel { UserId = 3, Username = "raymond.le", FullName = "quang", Email = "raymond.le@gmail.com", IsActive = true }
            };
            var userController = GetUserManagementController();
            baseService.GetDataAsync<UserModel>(Arg.Any<GetUsersCriteria>()).Returns(users);
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            // Act
            var actual = await userController.Index(UserStatus.Active);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var viewModel = Assert.IsAssignableFrom<UserManagementViewModel>(viewResult.Model);
            Assert.Null(viewResult.ViewName);
            Assert.True(viewModel.IsHost);
            Assert.Equal(users, viewModel.Users);
        }

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

        #region CreateUser

        [Fact]
        public void CreateUser_WhenCalled_ReturnExpectedViewResult()
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
        public async Task CreateUser_ModelStateIsInvalid_ReturnErrors()
        {
            // Arrange
            var model = new CreateUserViewModel();
            var userController = GetUserManagementController();
            userController.ModelState.AddModelError("", "Error");

            // Act
            var actual = await userController.CreateUser(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotEmpty(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value) as IEnumerable);
        }

        [Fact]
        public async Task CreateUser_ModelStateIsValid_ReturnExpectedCommandResult()
        {
            // Arrange
            var model = new CreateUserViewModel { Password = "Admin1234" };
            var expected = new CommandResult
            {
                Code = 0
            };
            var userController = GetUserManagementController();
            baseService.ExecuteNonQueryAsync(Arg.Any<CreateUserCriteria>()).Returns(Task.FromResult(expected));
            userController.Url = Substitute.For<IUrlHelper>();
            userController.Url.Action().ReturnsForAnyArgs("/UserManagement/AssignPermissions");

            // Act
            var actual = await userController.CreateUser(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("isSuccessful").GetValue(jsonResult.Value));
        }

        #endregion CreateUser

        #region EditUser

        [Fact]
        public async Task EditUser_ModelStateIsInvalid_ReturnErrors()
        {
            // Arrange
            var model = new EditUserViewModel();
            var userController = GetUserManagementController();
            userController.ModelState.AddModelError("", "Error");

            // Act
            var actual = await userController.EditUser(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.NotEmpty(jsonResult.Value.GetType().GetProperty("errors").GetValue(jsonResult.Value) as IEnumerable);
        }

        [Fact]
        public async Task EditUser_ModelStateIsValid_ReturnExpectedCommandResult()
        {
            // Arrange
            var model = new EditUserViewModel();
            var expected = new CommandResult
            {
                Code = 0
            };
            var userController = GetUserManagementController();
            baseService.ExecuteNonQueryAsync(Arg.Any<EditUserCriteria>()).Returns(Task.FromResult(expected));

            // Act
            var actual = await userController.EditUser(model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(actual);
            Assert.True((bool)jsonResult.Value.GetType().GetProperty("IsSuccessful").GetValue(jsonResult.Value));
        }

        [Fact]
        public async Task EditUser_ViewModelIsNull_ReturnRedirectToPageNotFound()
        {
            // Arrange
            var userController = GetUserManagementController();
            baseService.GetDataEntityAsync<EditUserViewModel>(Arg.Any<GetUserByIdCriteria>()).Returns(Task.FromResult<EditUserViewModel>(null));

            // Act
            var result = await userController.EditUser(1);

            // Assert
            var viewResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Error", viewResult.ControllerName);
            Assert.Equal("PageNotFound", viewResult.ActionName);
        }

        [Fact]
        public async Task EditUser_ViewModelHasValue_ReturnExpectedViewModel()
        {
            // Arrange
            var user = new EditUserViewModel { UserId = 1, Username = "Min", IsSystemAdmin = true };
            var userController = GetUserManagementController();
            userController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(new List<Claim>
                        {
                            new Claim(Constants.IsSystemAdmin, true.ToString())
                        })
                    )
                }
            };
            baseService.GetDataEntityAsync<EditUserViewModel>(Arg.Any<GetUserByIdCriteria>()).Returns(Task.FromResult(user));
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            // Act
            var actual = await userController.EditUser(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(actual);
            var model = Assert.IsAssignableFrom<EditUserViewModel>(viewResult.Model);
            Assert.True(model.CanEdit);
        }

        #endregion EditUser

        [Fact]
        public async Task CheckUsernameExisted_ExistedUserName_ReturnFalse()
        {
            // Arrange
            var username = "admin";
            var userController = GetUserManagementController();
            baseService.GetDataAsync<int>(Arg.Any<CheckUsernameExistedCriteria>()).Returns(new List<int> { 1 });

            // Act
            var actual = await userController.CheckUsernameExisted(username);

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