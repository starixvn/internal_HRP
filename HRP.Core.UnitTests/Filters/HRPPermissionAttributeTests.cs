namespace HRP.Core.UnitTests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Model;
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

        [Fact]
        public void OnAuthorization_UserIdIsValidAndHasPermissionOnFunction_ExpectedRequestSucceeded()
        {
            // Arrange
            var functionKey = "UserManagement";
            var session = new MockSession();
            session.SetInt32(SessionKey.InternalUserId, 1);
            session.Set<IEnumerable<FunctionModel>>(SessionKey.InternalFunctionList, new List<FunctionModel>
            {
                new FunctionModel { FunctionId = 1, FunctionKey = functionKey, IsRead = true }
            });
            var httpContext = new DefaultHttpContext
            {
                Session = session
            };
            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var authContext = new AuthorizationFilterContext(actionContext, Enumerable.Empty<IFilterMetadata>().ToList());
            var filter = new HRPPermissionAttribute
            {
                FunctionKey = functionKey,
                Policy = Policy.Read
            };

            // Act
            filter.OnAuthorization(authContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, authContext.HttpContext.Response.StatusCode);
        }
    }
}