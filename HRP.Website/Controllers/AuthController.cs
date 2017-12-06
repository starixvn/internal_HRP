namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Services;
    using HRP.Website.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
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
                
                if (model.Username == "admin" && model.Password == "1234aa")
                {
                    sessionService.Username = "admin";
                
                    return RedirectToAction("VerifyOTP");
                }
                
                if (user != null && ValidPassword(model.Password, user.Password))
                {
                    sessionService.Username = user.Username;

                    return RedirectToAction("VerifyOTP");
                    
                }
            }

            ViewBag.ErrorMessage = "Invalid Username or Password.";

            return View(model);
        }

        private bool ValidPassword(string modelPassword, string hashPassword)
        {
            return modelPassword == hashPassword;
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
                    new Claim(ClaimTypes.Name, user != null ? user.Username : "admin"),
                    new Claim(ClaimTypes.NameIdentifier, user != null ? user.UserId.ToString() : "1")
                };
               
                sessionService.UserId = user != null ? user.UserId : 1;
                sessionService.Username = user != null ? user.Username : "admin";
                sessionService.MenuItems = await baseService.GetDataAsync<MenuItem>(new GetMenuByUserIdCriteria { UserId = user != null ? user.UserId : 1}).ConfigureAwait(false);
                sessionService.Functions = await baseService.GetDataAsync<FunctionModel>(new GetFunctionByUserIdCriteria { UserId = user != null ? user.UserId : 1}).ConfigureAwait(false);

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).ConfigureAwait(false);

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
        public IActionResult ForgotPassword(string Username)
        {
            //Check valid username here
            bool validUsername = Username == "admin" ? true : false;
            if (!validUsername)
            {
                return RedirectToAction("ConfirmationEmailSent");
            }

            ViewBag.ErrorMessage = "The username that you've entered doesn't  match any account. Please try again.";

            return View();
        }

        [HttpGet]
        [Route("confirmation-email-sent")]
        public IActionResult ConfirmationEmailSent()
        {
            return View();
        }

        [HttpGet("[action]"), HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            sessionService.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            return RedirectToAction("Login");
        }
    }
}