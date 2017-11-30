namespace HRP_Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using HRP.Core.Services;
    using HRP.Services;
    using HRP.Models;
    using Google.Authenticator;

    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;

        public AuthController(
            IUserService userService,
            ISessionService sessionService)
        {
            _userService = userService;
            _sessionService = sessionService;
        }

        [HttpGet]
        public ActionResult Login()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var user = _userService.GetUserByUsername(model.Username);

                if (user != null)
                {
                    _sessionService.Username = user.Username;
                    
                    return RedirectToAction("VerifyOTP");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Route("verify-otp-code")]
        public ActionResult VerifyOTP()
        {
            if (!string.IsNullOrEmpty(_sessionService.Username))
            {
                TwoFactorAuthenticatorViewModel model = new TwoFactorAuthenticatorViewModel();
                model.SkipTwoFactor = true;
                return View(model);
            }
            return RedirectToAction("Login");
        }

        [HttpPost]
        [Route("verify-otp-code")]
        public async Task<IActionResult> VerifyOTP(TwoFactorAuthenticatorViewModel model)
        {
            var user = _userService.GetUserByUsername(_sessionService.Username);
            TwoFactorAuthenticator twoFactorAuthen = new TwoFactorAuthenticator();
            string SecretKey = "1234aa";
            DateTime nowDate = DateTime.Now;
            string UniqueKey = _sessionService.Username + SecretKey;
            string currentPIN = twoFactorAuthen.GetCurrentPIN(SecretKey, nowDate);
            bool isValidOTP = twoFactorAuthen.ValidateTwoFactorPIN(UniqueKey, model.VerifyCode);
            bool stepTwo = (ModelState.IsValid && isValidOTP) || (model.SkipTwoFactor);

            if (stepTwo)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                };

                _sessionService.UserId = user.UserId;
                _sessionService.Username = user.Username;
                _sessionService.MenuItems = user.MenuItems;
                _sessionService.Functions = user.Functions;

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Redirect("~/Home");
                
            }
            _sessionService.ClearAll();
            return RedirectToAction("VerifyOTP");
        }

        [HttpGet("[action]"), HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            _sessionService.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}
