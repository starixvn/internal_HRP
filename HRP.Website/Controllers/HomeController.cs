namespace HRP_Website.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Core.Services;

    [HRPAuthorize]
    public class HomeController : Controller
    {
        private readonly ISessionService _sessionService;

        public HomeController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }

        public IEnumerable<MenuItem> GetMenuByCurrentUser() 
        {
            return _sessionService.MenuItems;
        }
    }
}