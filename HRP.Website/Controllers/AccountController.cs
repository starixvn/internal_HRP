namespace HRP.Website.Controllers
{
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Services;
    using Microsoft.AspNetCore.Mvc;
    using HRP.Models;

    [HRPAuthorize]
    public class AccountController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly ITwoFactorAuthenticatorService twoFactorAuthService;

        public AccountController(ISessionService sessionService, ITwoFactorAuthenticatorService twoFactorAuthService)
        {
            this.sessionService = sessionService;
            this.twoFactorAuthService = twoFactorAuthService;
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var model = new UserProfileViewModel
            {
                //QRImageUrl = twoFactorAuthService.GenerateQrImageUrl(sessionService.Username, "1234aa"),
                //SetupCode = twoFactorAuthService.GenerateSetupCode(sessionService.Username, "1234aa")
            };

            return View(model);
        }
    }
}