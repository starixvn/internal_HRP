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
    using HRP.Services.Interfaces;

    public class AuthController : Controller
    {
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;
        private readonly ITwoFactorAuthenticatorService _twoFactorAuthService;

        public AuthController(
            IUserService userService,
            ISessionService sessionService,
            ITwoFactorAuthenticatorService twoFactorAuthenService)
        {
            _userService = userService;
            _sessionService = sessionService;
            _twoFactorAuthService = twoFactorAuthenService;
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
        [Route("verify-otp")]
        public ActionResult VerifyOTP()
        {
            if (!string.IsNullOrEmpty(_sessionService.Username))
            {
                var model = new TwoFactorAuthenticatorViewModel
                {
                    SkipTwoFactor = true
                };

                return View(model);
            }

            return RedirectToAction("Login");
        }

        [HttpPost]
        [Route("verify-otp")]
        public async Task<IActionResult> VerifyOTP(TwoFactorAuthenticatorViewModel model)
        {
            var user = _userService.GetUserByUsername(_sessionService.Username);
            //temporary secret_key is 1234aa
            bool isValidOTP = _twoFactorAuthService.ValidateOTPCode(_sessionService.Username,"1234aa", model.OTPCode);
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
            
            return RedirectToAction("VerifyOTP");
        }


        [HttpGet]
        [Route("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
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
