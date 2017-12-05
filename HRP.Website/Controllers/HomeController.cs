namespace HRP.Website.Controllers
{
    using System.Diagnostics;
    using Google.Authenticator;
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Models;
    using HRP.Website.Models;
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

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var twoFactorAuthen = new TwoFactorAuthenticator();
            string SecretKey = "1234aa";
            string UniqueKey = sessionService.Username + SecretKey;
            var setupInfo = twoFactorAuthen.GenerateSetupCode("HRP Authenticator", sessionService.Username, UniqueKey, 200, 200);
            var model = new UserProfileViewModel
            {
                QRImageUrl = setupInfo.QrCodeSetupImageUrl,
                SetupCode = setupInfo.ManualEntryKey
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult AssignFunction()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}