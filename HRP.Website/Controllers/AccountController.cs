namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Models;
    using HRP.Core.Services;
    using HRP.Services;

    [HRPAuthorize]
    public class AccountController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ITwoFactorAuthenticatorService _twoFactorAuthService;

        public AccountController(ISessionService sessionService, ITwoFactorAuthenticatorService twoFactorAuthService)
        {
            _sessionService = sessionService;
            _twoFactorAuthService = twoFactorAuthService;
        }

        [HttpGet]
        public IActionResult Profile()
        {
            UserProfileViewModel model = new UserProfileViewModel();
            model.QRImageUrl = _twoFactorAuthService.GenerateQrImageUrl(_sessionService.Username, "1234aa");
            model.SetupCode = _twoFactorAuthService.GenerateSetupCode(_sessionService.Username, "1234aa"); 
            return View(model);
        }

        [HttpGet]
        public bool IsAuthenticated()
        {
            return _sessionService.UserId > 0 && !string.IsNullOrWhiteSpace(_sessionService.Username);
        }
    }
}