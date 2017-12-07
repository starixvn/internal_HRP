namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using BCrypt.Net;
    using HRP.Resources;
    using HRP.Criteria.User;

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

                if (user != null && ValidPassword(model.Password, user.Password))
                {
                    sessionService.Username = user.Username;

                    return RedirectToAction("VerifyOTP");
                }
            }

            ViewBag.ErrorMessage = Message.LoginInvalid;

            return View(model);
        }

        private bool ValidPassword(string modelPassword, string hashedPassword)
        {
            string password = BCrypt.HashPassword(modelPassword);
            return BCrypt.Verify(modelPassword, hashedPassword);
        }

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
        public async Task<IActionResult> VerifyOTP(TwoFactorAuthenticatorViewModel model)
        {
            var user = userService.GetUserByUsername(sessionService.Username);
            bool isValidOTP = twoFactorAuthService.ValidateOTPCode(sessionService.Username, user.SecretKey, model.OTPCode);
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
                sessionService.MenuItems = await baseService.GetDataAsync<MenuItem>(new GetMenuByUserIdCriteria { UserId = user.UserId }).ConfigureAwait(false);
                sessionService.Functions = await baseService.GetDataAsync<FunctionModel>(new GetFunctionByUserIdCriteria { UserId = user.UserId }).ConfigureAwait(false);

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).ConfigureAwait(false);

                return Redirect("~/Home");
            }

            return RedirectToAction("VerifyOTP");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string username)
        {
            //Check valid username here
            var user = userService.GetUserByUsername(username);

            if (user != null)
            {
                userService.ResetUserPassword(username);

                return RedirectToAction("ConfirmationEmailSent");
            }

            ViewBag.ErrorMessage = Message.ForgotPasswordInvalidUser;

            return View();
        }

        [Route("reset-password")]
        public IActionResult ResetPassword(string token)
        {
            var user = userService.GetUserByTokenResetPassword(token);

            if (user == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        [Route("reset-password")]
        public IActionResult ResetPassword(string token, ChangePasswordViewModel model)
        {
            var user = userService.GetUserByTokenResetPassword(token);

            if (model != null && ModelState.IsValid && user != null)
            {
                var criteria = new UpdateUserPasswordCriteria
                {
                    UserId = user.UserId,
                    Password = BCrypt.HashPassword(model.Password)
                };

                CommandResult result = baseService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
            }
            return BadRequest();
        }

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