namespace HRP.Core.UnitTests
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using HRP.Core.CustomException;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using Xunit;

    public class ErrorHandlingMiddlewareTests
    {
        [Fact]
        public async Task Invoke_DefaultException_RedirectToInternalError()
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
            Assert.Equal((int)HttpStatusCode.Found, context.Response.StatusCode);
            Assert.Equal("/error/500", context.Response.Headers.Values.First().ToString());
        }

        [Fact]
        public async Task Invoke_CertificateNotFoundException_RedirectToInternalError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var logger = Substitute.For<ILogger<ErrorHandlingMiddleware>>();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(Arg.Any<HttpContext>()).Returns(x => { throw new CertificateNotFoundException(); });
            var middleware = new ErrorHandlingMiddleware(next, logger);

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Found, context.Response.StatusCode);
            Assert.Equal("/SystemSecurity/FileNotFound", context.Response.Headers.Values.First().ToString());
        }

        [Fact]
        public async Task Invoke_CertificatePasswordException_RedirectToInternalError()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var logger = Substitute.For<ILogger<ErrorHandlingMiddleware>>();
            var next = Substitute.For<RequestDelegate>();
            next.Invoke(Arg.Any<HttpContext>()).Returns(x => { throw new CertificatePasswordException(); });
            var middleware = new ErrorHandlingMiddleware(next, logger);

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.Found, context.Response.StatusCode);
            Assert.Equal("/SystemSecurity/password", context.Response.Headers.Values.First().ToString());
        }

        [Fact]
        public async Task Invoke_RequestSucceeded_ReturnAsExpected()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var logger = Substitute.For<ILogger<ErrorHandlingMiddleware>>();
            var next = Substitute.For<RequestDelegate>();
            var middleware = new ErrorHandlingMiddleware(next, logger);

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
        }
    }
}