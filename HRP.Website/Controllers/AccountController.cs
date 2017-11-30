namespace HRP_Website.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Models;
    using HRP.Core.Services;
    using Google.Authenticator;

    [HRPAuthorize]
    public class AccountController : Controller
    {
        private readonly ISessionService _sessionService;

        public AccountController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Profile()
        {
            TwoFactorAuthenticator twoFactorAuthen = new TwoFactorAuthenticator();
            string SecretKey = "1234aa";
            string UniqueKey = _sessionService.Username + SecretKey;
            var setupInfo = twoFactorAuthen.GenerateSetupCode("HRP Authenticator", _sessionService.Username, UniqueKey, 200, 200);
            UserProfileViewModel model = new UserProfileViewModel();
            model.QRImageUrl = setupInfo.QrCodeSetupImageUrl;
            model.SetupCode = setupInfo.ManualEntryKey; 

            return View(model);
        }

        [HttpGet]
        public bool IsAuthenticated()
        {
            return _sessionService.UserId > 0 && !string.IsNullOrWhiteSpace(_sessionService.Username);
        }
    }
}