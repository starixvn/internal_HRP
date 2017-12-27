namespace HRP.Core.UnitTests
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Xunit;

    public class ErrorHandlingMiddlewareTests
    {
        [Fact]
        public async Task Invoke_ThrowException_RedirectToInternalError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var logger = Substitute.For<ILogger<ErrorHandlingMiddleware>>();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(Arg.Any<HttpContext>()).Returns(x => { throw new Exception(); });
            var middleware = new ErrorHandlingMiddleware(next, logger);

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal("/error/500", context.Response.Headers.Values.First().ToString());
        }
    }
}