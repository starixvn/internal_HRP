namespace HRP.Core.UnitTests
{
    using System;
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Razor;
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
            ViewEngineResult viewEngineResult = null;
            var service = GetViewRenderService();
            razorViewEngine.FindView(Arg.Any<ActionContext>(), Arg.Any<string>(), false).Returns(viewEngineResult);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => service.RenderViewToStringAsync("Index", 1));
            Assert.Equal("Value cannot be null.\r\nParameter name: Index does not match any available view", ex.Message);
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