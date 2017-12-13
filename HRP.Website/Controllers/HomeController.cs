namespace HRP.Website.Controllers
{
    using Google.Authenticator;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class HomeController : Controller
    {
        private readonly ISessionService sessionService;

        public HomeController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}