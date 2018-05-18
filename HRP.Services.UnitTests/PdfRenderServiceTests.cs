namespace HRP.Services.UnitTests
{
    using Microsoft.AspNetCore.Hosting;
    using NSubstitute;
    using Xunit;

    public class PdfRenderServiceTests
    {
        private IHostingEnvironment hostingEnvironment;

        [Fact]
        public void RenderContent_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var service = GetPdfRenderService();

            // Act
            var actual = service.RenderContent(@"FileTest\\pdf.min.css", "<h1>hello</h1>");

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void RenderContentWithPasswordProtected_WhenCalled_ReturnAsExpected()
        {
            // Arrange
            var service = GetPdfRenderService();

            // Act
            var actual = service.RenderContentWithPasswordProtected(@"FileTest\\pdf.min.css", "<h1>hello</h1>", "123456", "123456");

            // Assert
            Assert.NotNull(actual);
        }

        #region Helpers

        private static IHostingEnvironment FakeHostingEnvironment()
        {
            return Substitute.For<IHostingEnvironment>();
        }

        private PdfRenderService GetPdfRenderService()
        {
            hostingEnvironment = FakeHostingEnvironment();
            var service = new PdfRenderService(hostingEnvironment);

            return service;
        }

        #endregion Helpers
    }
}