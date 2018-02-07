namespace HRP.Core.UnitTests
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Routing;
    using Xunit;

    public class MockUrlHelper : IUrlHelper
    {
        private readonly string _routeName;
        private readonly string _url;

        public MockUrlHelper(string url, string routeName)
        {
            _routeName = routeName;
            _url = url;
        }

        public ActionContext ActionContext { get; }

        public object RouteValues { get; private set; }

        public string Action(UrlActionContext actionContext)
        {
            throw new NotImplementedException();
        }

        public string Content(string contentPath)
        {
            throw new NotImplementedException();
        }

        public bool IsLocalUrl(string url)
        {
            throw new NotImplementedException();
        }

        public string Link(string routeName, object values)
        {
            throw new NotImplementedException();
        }

        public string RouteUrl(UrlRouteContext routeContext)
        {
            Assert.Equal(_routeName, routeContext.RouteName);
            Assert.Null(routeContext.Protocol);
            Assert.Null(routeContext.Host);
            Assert.Null(routeContext.Fragment);

            RouteValues = routeContext.Values;

            return _url;
        }
    }
}