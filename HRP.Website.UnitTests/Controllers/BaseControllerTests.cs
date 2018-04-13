namespace HRP.Website.UnitTests.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Core.Criteria;
    using Core.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;
    using Website.Controllers;
    using Xunit;

    public class BaseControllerTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;

        [Fact]
        public void OnActionExecuting_IsKickedOut_ExpectedRedirectToLogin()
        {
            // Arrange
            var authService = Substitute.For<IAuthenticationService>();
            var serviceProvider = Substitute.For<IServiceProvider>();
            serviceProvider.GetService(typeof(IAuthenticationService)).Returns(authService);
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };

            var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Substitute.For<Controller>()
            );

            actionContext.HttpContext.Connection.RemoteIpAddress = IPAddress.Any;
            actionContext.HttpContext.Request.Headers["User-Agent"] = "12345";
            var controller = GetBaseController();
            baseService.GetDataEntityAsync<bool>(Arg.Any<ReCheckInUserCriteria>()).Returns(true);

            // Act
            controller.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.Equal("/auth/login", actionExecutingContext.HttpContext.Response.Headers.Values.First().ToString());
            Assert.Equal((int)HttpStatusCode.Found, actionExecutingContext.HttpContext.Response.StatusCode);
        }

        [Fact]
        public void OnActionExecuting_IsNotKickedOut_ExpectedRequestSucceeded()
        {
            // Arrange
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Substitute.For<Controller>()
            );
            var controller = GetBaseController();
            baseService.GetDataEntityAsync<bool>(Arg.Any<ReCheckInUserCriteria>()).Returns(false);

            // Act
            controller.OnActionExecuting(actionExecutingContext);

            // Assert
            Assert.Equal((int)HttpStatusCode.OK, actionExecutingContext.HttpContext.Response.StatusCode);
        }

        #region Helpers

        private static ISessionService FakeSessionService()
        {
            var service = Substitute.For<ISessionService>();
            service.UserId = 1;

            return service;
        }

        private static IBaseService FakeBaseService()
        {
            var service = Substitute.For<IBaseService>();

            return service;
        }

        private BaseController GetBaseController()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            var controller = new BaseController(baseService, sessionService);

            return controller;
        }

        #endregion Helpers
    }
}