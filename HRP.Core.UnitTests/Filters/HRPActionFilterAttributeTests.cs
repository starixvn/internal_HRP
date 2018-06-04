namespace HRP.Core.UnitTests.Filters
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using HRP.Core.Criteria;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;
    using NSubstitute;
    using Xunit;

    public class HRPActionFilterAttributeTests
    {
        private IBaseService baseService;
        private ISessionService sessionService;
        private ICertificateService certificateService;

        [Fact]
        public void OnActionExecuting_IsKickedOut_ExpectedRedirectToLogout()
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

            var actionFilter = GetActionFilter();
            baseService.GetDataEntityAsync<bool>(Arg.Any<ReCheckInUserCriteria>()).Returns(true);

            // Act
            actionFilter.OnActionExecuting(actionExecutingContext);

            // Assert
            var redirectResult = Assert.IsAssignableFrom<RedirectToActionResult>(actionExecutingContext.Result);
            Assert.Equal("auth", redirectResult.ControllerName);
            Assert.Equal("logout", redirectResult.ActionName);
        }

        [Fact]
        public void OnActionExecuting_CertificateIsNotGenerated_ExpectedRedirectToGenerateCertificate()
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

            var actionFilter = GetActionFilter();
            baseService.GetDataEntityAsync<bool>(Arg.Any<ReCheckInUserCriteria>()).Returns(false);
            certificateService.IsGeneratedCertificate().Returns(false);

            // Act
            actionFilter.OnActionExecuting(actionExecutingContext);

            // Assert
            var redirectResult = Assert.IsAssignableFrom<RedirectToActionResult>(actionExecutingContext.Result);
            Assert.Equal("SystemSecurity", redirectResult.ControllerName);
            Assert.Equal("generate", redirectResult.ActionName);
        }

        [Fact]
        public void OnActionExecuting_CertificateIsNotExisted_ExpectedRedirectToFileNotFound()
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

            var actionFilter = GetActionFilter();
            baseService.GetDataEntityAsync<bool>(Arg.Any<ReCheckInUserCriteria>()).Returns(false);
            certificateService.IsGeneratedCertificate().Returns(true);
            certificateService.IsExistedCertificate().Returns(false);

            // Act
            actionFilter.OnActionExecuting(actionExecutingContext);

            // Assert
            var redirectResult = Assert.IsAssignableFrom<RedirectToActionResult>(actionExecutingContext.Result);
            Assert.Equal("SystemSecurity", redirectResult.ControllerName);
            Assert.Equal("FileNotFound", redirectResult.ActionName);
        }

        [Fact]
        public void OnActionExecuting_PasswordIsNull_ExpectedRedirectToInputPassword()
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

            var actionFilter = GetActionFilter();
            baseService.GetDataEntityAsync<bool>(Arg.Any<ReCheckInUserCriteria>()).Returns(false);
            certificateService.IsGeneratedCertificate().Returns(true);
            certificateService.IsExistedCertificate().Returns(true);
            certificateService.GetPasswordAccessTheCertificate().Returns(x => null);

            // Act
            actionFilter.OnActionExecuting(actionExecutingContext);

            // Assert
            var redirectResult = Assert.IsAssignableFrom<RedirectToActionResult>(actionExecutingContext.Result);
            Assert.Equal("SystemSecurity", redirectResult.ControllerName);
            Assert.Equal("password", redirectResult.ActionName);
        }

        [Fact]
        public void OnActionExecuting_PasswordIsIncorrect_ExpectedRedirectToInputPassword()
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

            var actionFilter = GetActionFilter();
            baseService.GetDataEntityAsync<bool>(Arg.Any<ReCheckInUserCriteria>()).Returns(false);
            certificateService.IsGeneratedCertificate().Returns(true);
            certificateService.IsExistedCertificate().Returns(true);
            certificateService.GetPasswordAccessTheCertificate().Returns("aa");
            certificateService.IsCorrectPasswordAccessTheCertificate("aa").Returns(false);

            // Act
            actionFilter.OnActionExecuting(actionExecutingContext);

            // Assert
            var redirectResult = Assert.IsAssignableFrom<RedirectToActionResult>(actionExecutingContext.Result);
            Assert.Equal("SystemSecurity", redirectResult.ControllerName);
            Assert.Equal("password", redirectResult.ActionName);
        }

        [Fact]
        public void OnActionExecuting_WhenCalled_ExpectedRequestSucceeded()
        {
            // Arrange
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var actionExecutingContext = new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Substitute.For<Controller>()
            );
            var actionFilter = GetActionFilter();
            baseService.GetDataEntityAsync<bool>(Arg.Any<ReCheckInUserCriteria>()).Returns(false);
            certificateService.IsGeneratedCertificate().Returns(true);
            certificateService.IsExistedCertificate().Returns(true);
            certificateService.GetPasswordAccessTheCertificate().Returns("aa");
            certificateService.IsCorrectPasswordAccessTheCertificate("aa").Returns(true);

            // Act
            actionFilter.OnActionExecuting(actionExecutingContext);

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

        private static ICertificateService FakeCertificateService()
        {
            var service = Substitute.For<ICertificateService>();

            return service;
        }

        private HRPActionFilterAttribute GetActionFilter()
        {
            baseService = FakeBaseService();
            sessionService = FakeSessionService();
            certificateService = FakeCertificateService();

            return new HRPActionFilterAttribute(baseService, sessionService, certificateService);
        }

        #endregion Helpers
    }
}