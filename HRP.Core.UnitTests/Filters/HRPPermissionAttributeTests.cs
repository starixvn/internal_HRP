namespace HRP.Core.UnitTests
{
    using System.Linq;
    using System.Net;
    using HRP.Core.Filters;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using Xunit;

    public class HRPPermissionAttributeTests
    {
        [Fact]
        public void OnAuthorization_UserIdIsInvalid_ExpectedRedirectToAccessDenied()
        {
            // Arrange
            var httpContext = new DefaultHttpContext
            {
                Session = new MockSession()
            };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var authContext = new AuthorizationFilterContext(actionContext, Enumerable.Empty<IFilterMetadata>().ToList());
            var filter = new HRPPermissionAttribute();

            // Act
            filter.OnAuthorization(authContext);

            // Assert
            Assert.Equal("/error/405", authContext.HttpContext.Response.Headers.Values.First().ToString());
            Assert.Equal((int)HttpStatusCode.Found, authContext.HttpContext.Response.StatusCode);
        }
    }
}