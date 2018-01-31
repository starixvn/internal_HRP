namespace HRP.Core.UnitTests.Filters
{
    using System;
    using System.Linq;
    using System.Net;
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
        public void OnAuthorization_UserIdAndUsernameAreInvalid_ExpectedRedirectToAccessDenied()
        {
            // Arrange
            var authService = Substitute.For<IAuthenticationService>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IAuthenticationService)).Returns(authService);
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider,
                Session = new MockSession()
            };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var authContext = new AuthorizationFilterContext(actionContext, Enumerable.Empty<IFilterMetadata>().ToList());
            var filter = new HRPAuthorizeAttribute();

            // Act
            filter.OnAuthorization(authContext);

            // Assert
            Assert.Equal("/auth/login", authContext.HttpContext.Response.Headers.Values.First().ToString());
            Assert.Equal((int)HttpStatusCode.Found, authContext.HttpContext.Response.StatusCode);
        }
    }
}