namespace HRP.Website.UnitTests.ViewComponents
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Website.ViewComponents;
    using Microsoft.AspNetCore.Mvc.ViewComponents;
    using NSubstitute;
    using Xunit;

    public class MenuViewComponentTests
    {
        private ISessionService sessionService;
        private MenuViewComponent menuViewComponent;

        [Fact]
        public async Task InvokeAsync_CheckViewName_ReturnAsExpected()
        {
            // Arrange
            var expected = "Menu";
            menuViewComponent = GetMenuViewComponent();

            // Act
            var actual = await menuViewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(actual);
            Assert.Equal(expected, viewResult.ViewName);
        }

        [Fact]
        public async Task InvokeAsync_CheckViewData_ReturnAsExpected()
        {
            // Arrange
            var username = "tim.trinh";
            var menus = new List<MenuItem>()
            {
                new MenuItem { MenuId = 1, Name = "Configuration" }
            };
            menuViewComponent = GetMenuViewComponent();
            sessionService.Username.Returns(username);
            sessionService.MenuItems.Returns(menus);

            // Act
            var actual = await menuViewComponent.InvokeAsync();

            // Assert
            var viewResult = Assert.IsType<ViewViewComponentResult>(actual);
            var viewModel = Assert.IsAssignableFrom<List<MenuItem>>(viewResult.ViewData.Model);
            Assert.Equal(username, viewResult.ViewData["Username"]);
            Assert.Equal(menus, viewModel);
        }

        #region Helpers

        private ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();

            return service;
        }

        private MenuViewComponent GetMenuViewComponent()
        {
            sessionService = FakeSessionService();
            var viewComponent = new MenuViewComponent(sessionService);

            return viewComponent;
        }

        #endregion Helpers
    }
}