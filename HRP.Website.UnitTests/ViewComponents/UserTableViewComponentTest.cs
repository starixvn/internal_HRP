namespace HRP.Website.UnitTests.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly IList<UserModel> users = new List<UserModel>
        {
            new UserModel { UserId = 1, IsActive = true },
            new UserModel { UserId = 2, IsActive = false },
            new UserModel { UserId = 3, IsActive = true }
        };

        private readonly ISessionService sessionService = Substitute.For<ISessionService>();
        private readonly IBaseService baseService = Substitute.For<IBaseService>();

        private UserTableViewComponent GetMenuViewComponent()
        {
            var viewComponent = new UserTableViewComponent(sessionService, baseService);

            return viewComponent;
        }

        [Fact]
        public async Task InvokeAsync_AllStatuses_ReturnAllUsers()
        {
            var viewComponent = GetMenuViewComponent();
            baseService.GetDataAsync<UserModel>(Arg.Any<GetUsersCriteria>()).Returns(users);
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            var actual = await viewComponent.InvokeAsync(null);

            var viewResult = Assert.IsType<ViewViewComponentResult>(actual);
            var viewModel = Assert.IsAssignableFrom<IEnumerable<UserModel>>(viewResult.ViewData.Model);

            Assert.Equal(users.Count, viewModel.Count());
        }

        [Fact]
        public async Task InvokeAsync_ActiveStatus_ReturnActiveUsers()
        {
            var viewComponent = GetMenuViewComponent();
            baseService.GetDataAsync<UserModel>(Arg.Any<GetUsersCriteria>()).Returns(users.Where(x => x.IsActive));
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            var actual = await viewComponent.InvokeAsync(null);

            var viewResult = Assert.IsType<ViewViewComponentResult>(actual);
            var viewModel = Assert.IsAssignableFrom<IEnumerable<UserModel>>(viewResult.ViewData.Model);

            Assert.True(viewModel.All(x => x.IsActive));
        }

        [Fact]
        public async Task InvokeAsync_InactiveStatus_ReturnInactiveUsers()
        {
            var viewComponent = GetMenuViewComponent();
            baseService.GetDataAsync<UserModel>(Arg.Any<GetUsersCriteria>()).Returns(users.Where(x => !x.IsActive));
            sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host).Returns(true);

            var actual = await viewComponent.InvokeAsync(null);

            var viewResult = Assert.IsType<ViewViewComponentResult>(actual);
            var viewModel = Assert.IsAssignableFrom<IEnumerable<UserModel>>(viewResult.ViewData.Model);

            Assert.True(viewModel.All(x => !x.IsActive));
        }
    }
}