namespace HRP.Website.UnitTests.ViewComponents
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Website.ViewComponents;
    using Microsoft.AspNetCore.Mvc.ViewComponents;
    using NSubstitute;
    using Xunit;

    public class UserTableViewComponentTest
    {
        private ISessionService sessionService;
        private IBaseService baseService;

        [Fact]
        public async Task InvokeAsync_CheckViewName_ReturnAsExpected()
        {
            // Arrange
            var expected = "UserTable";
            var viewComponent = GetUserTableViewComponent();

            // Act
            var actual = await viewComponent.InvokeAsync(null);

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(actual);
            Assert.Equal(expected, viewResult.ViewName);
        }

        [Fact]
        public async Task InvokeAsync_CheckViewBag_ReturnAsExpected()
        {
            // Arrange
            var canEdit = true;
            var viewComponent = GetUserTableViewComponent();
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(canEdit);

            // Act
            var actual = await viewComponent.InvokeAsync(null);

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(actual);
            Assert.Equal(canEdit, viewResult.ViewData["CanEdit"]);
        }

        [Fact]
        public async Task InvokeAsync_WhenCalled_ReturnAsExpectedUsers()
        {
            // Arrange
            var users = new List<UserModel>
            {
                new UserModel { UserId = 1, Username = "tim.trinh", FullName = "thang", Email = "tim.trinh@gmail.com", IsActive = true },
                new UserModel { UserId = 2, Username = "hunter.tran", FullName = "tuan", Email = "hunter.tran@gmail.com", IsActive = false },
                new UserModel { UserId = 3, Username = "raymond.le", FullName = "quang", Email = "raymond.le@gmail.com", IsActive = true }
            };
            var viewComponent = GetUserTableViewComponent();
            baseService.GetDataAsync<UserModel>(Arg.Any<GetUsersCriteria>()).Returns(users);
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            // Act
            var actual = await viewComponent.InvokeAsync(null);

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(actual);
            var viewModel = Assert.IsAssignableFrom<IEnumerable<UserModel>>(viewResult.ViewData.Model);
            Assert.Equal(users, viewModel);
        }

        private UserTableViewComponent GetUserTableViewComponent()
        {
            sessionService = Substitute.For<ISessionService>();
            baseService = Substitute.For<IBaseService>();
            var viewComponent = new UserTableViewComponent(sessionService, baseService);

            return viewComponent;
        }
    }
}