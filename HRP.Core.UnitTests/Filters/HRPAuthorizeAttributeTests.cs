namespace HRP.Core.UnitTests.Filters
{
    using System;
    using System.Linq;
    using System.Net;
    using HRP.Core.Constants;
    using HRP.Core.Filters;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;
    using Xunit;

    public class HRPAuthorizeAttributeTests
    {
        [Fact]
        public void OnAuthorization_ContextIsNull_ThrowException()
        {
            // Arrange
            var filter = new HRPAuthorizeAttribute();

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => filter.OnAuthorization(null));
            Assert.Equal("Value cannot be null.\r\nParameter name: context", ex.Message);
        }

        [Theory]
        [InlineData(1, "")]
        [InlineData(0, "tim.trinh")]
        [InlineData(0, "")]
        public void OnAuthorization_UserIdAndUsernameAreInvalid_ExpectedRedirectToAccessDenied(int userId, string username)
        {
            // Arrange
            var authService = Substitute.For<IAuthenticationService>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IAuthenticationService)).Returns(authService);

            var session = new MockSession();
            session.SetInt32(SessionKey.InternalUserId, userId);
            session.SetString(SessionKey.InternalUsername, username);
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider,
                Session = session
            };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var authContext = new AuthorizationFilterContext(actionContext, Enumerable.Empty<IFilterMetadata>().ToList());
            var filter = new HRPAuthorizeAttribute();

            // Act
            filter.OnAuthorization(authContext);

            // Assert
            var redirectResult = Assert.IsAssignableFrom<RedirectToActionResult>(authContext.Result);
            Assert.Equal("auth", redirectResult.ControllerName);
            Assert.Equal("logout", redirectResult.ActionName);
        }

        [Fact]
        public void OnAuthorization_UserIdAndUsernameAreValid_RequestSucceeded()
        {
            var authService = Substitute.For<IAuthenticationService>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IAuthenticationService)).Returns(authService);

            var session = new MockSession();
            session.SetInt32(SessionKey.InternalUserId, 1);
            session.SetString(SessionKey.InternalUsername, "tim.trinh");
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider,
                Session = session
            };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var authContext = new AuthorizationFilterContext(actionContext, Enumerable.Empty<IFilterMetadata>().ToList());
            var filter = new HRPAuthorizeAttribute();

            // Act
            filter.OnAuthorization(authContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, authContext.HttpContext.Response.StatusCode);
        }
    }
}