namespace HRP.Core.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Razor;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewEngines;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    using NSubstitute;
    using Xunit;

    public class ViewRenderServiceTests
    {
        private IRazorViewEngine razorViewEngine;
        private ITempDataProvider tempDataProvider;
        private IServiceProvider serviceProvider;

        [Fact]
        public async Task RenderViewToStringAsync_ViewIsNotExist_ThrowException()
        {
            // Arrange
            var viewName = "Index";
            var view = Substitute.For<IView>();
            view.RenderAsync(Arg.Any<ViewContext>()).Returns(Task.FromResult(true));
            var viewEngineResult = ViewEngineResult.NotFound(viewName, Enumerable.Empty<string>());
            var service = GetViewRenderService();
            razorViewEngine.FindView(Arg.Any<ActionContext>(), Arg.Any<string>(), false).Returns(viewEngineResult);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.RenderViewToStringAsync(viewName, 1));
            Assert.Equal("Index does not match any available view", ex.Message);
        }

        [Fact]
        public async Task RenderViewToStringAsync_ViewIsExist_ReturnAsExpected()
        {
            // Arrange
            var viewName = "Index";
            var view = Substitute.For<IView>();
            view.RenderAsync(Arg.Any<ViewContext>()).Returns(Task.FromResult(true));
            var viewEngineResult = ViewEngineResult.Found(viewName, view);
            var service = GetViewRenderService();
            razorViewEngine.FindView(Arg.Any<ActionContext>(), Arg.Any<string>(), false).Returns(viewEngineResult);

            // Act
            await service.RenderViewToStringAsync(viewName, 1);

            // Assert
            await view.Received().RenderAsync(Arg.Any<ViewContext>());
        }

        #region Helpers

        private static IRazorViewEngine FakeRazorViewEngine()
        {
            return Substitute.For<IRazorViewEngine>();
        }

        private static ITempDataProvider FakeTempDataProvider()
        {
            return Substitute.For<ITempDataProvider>();
        }

        private static IServiceProvider FakeServiceProvider()
        {
            return Substitute.For<IServiceProvider>();
        }

        private ViewRenderService GetViewRenderService()
        {
            razorViewEngine = FakeRazorViewEngine();
            tempDataProvider = FakeTempDataProvider();
            serviceProvider = FakeServiceProvider();

            return new ViewRenderService(razorViewEngine, tempDataProvider, serviceProvider);
        }

        #endregion Helpers
    }
}