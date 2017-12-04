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
    using HRP.Core.Model;
    using HRP.Criteria;
    using HRP.Services;
    using HRP.Models;

    public class AuthController : Controller
    {
        private readonly IUserService userService;
        private readonly ISessionService sessionService;
        private readonly ITwoFactorAuthenticatorService twoFactorAuthService;
        private readonly IBaseService baseService;

        public AuthController(
            IUserService userService,
            ISessionService sessionService,
            ITwoFactorAuthenticatorService twoFactorAuthenService,
            IBaseService baseService)
        {
            this.userService = userService;
            this.sessionService = sessionService;
            this.twoFactorAuthService = twoFactorAuthenService;
            this.baseService = baseService;
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
                var user = userService.GetUserByUsername(model.Username);

                if (user != null)
                {
                    sessionService.Username = user.Username;
                    
                    return RedirectToAction("VerifyOTP");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Route("verify-otp")]
        public ActionResult VerifyOTP()
        {
            if (!string.IsNullOrEmpty(sessionService.Username))
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
            var user = userService.GetUserByUsername(sessionService.Username);
            //temporary secret_key is 1234aa
            bool isValidOTP = twoFactorAuthService.ValidateOTPCode(sessionService.Username, "1234aa", model.OTPCode);
            bool stepTwo = (ModelState.IsValid && isValidOTP) || model.SkipTwoFactor;

            if (stepTwo)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                };

                sessionService.UserId = user.UserId;
                sessionService.Username = user.Username;
                sessionService.MenuItems = await baseService.GetDataAsync<MenuItem>(new GetMenuByUserIdCriteria { UserId = user.UserId });
                sessionService.Functions = await baseService.GetDataAsync<FunctionModel>(new GetFunctionByUserIdCriteria { UserId = user.UserId });

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

        [HttpPost]
        [Route("forgot-password")]
        public IActionResult ForgotPassword([FromBody] string username)
        {
            return View(username);
        }

        [HttpGet("[action]"), HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            sessionService.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }
    }
}
