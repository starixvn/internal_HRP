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
    }
}