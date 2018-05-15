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
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.RenderViewToStringAsync("Index", 1));
            Assert.Equal("Index does not match any available view", ex.Message);
        }

        ////[Fact]
        ////public async Task RenderViewToStringAsync_ViewIsExist_ReturnAsExpected()
        ////{
        ////    // Arrange
        ////    var viewHtmlContent = "hello";
        ////    var resultView = Substitute.For<IView>();
        ////    resultView.When(x => x.RenderAsync(Arg.Any<ViewContext>())).Do(c =>
        ////    {
        ////        var textWriter = c.Arg<TextWriter>();
        ////        textWriter.Write(viewHtmlContent);
        ////    });
        ////    var viewEngineResult = Substitute.For<ViewEngineResult>();
        ////    viewEngineResult.View.Returns(resultView);
        ////    var service = GetViewRenderService();
        ////    razorViewEngine.FindView(Arg.Any<ActionContext>(), Arg.Any<string>(), false).Returns(viewEngineResult);

        ////    // Act
        ////    var actual = await service.RenderViewToStringAsync("Index", 1);

        ////    // Assert
        ////    Assert.Equal(viewHtmlContent, actual);
        ////}

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