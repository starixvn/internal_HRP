namespace HRP.Website.Controllers
{
    using System.Threading.Tasks;
    using BCrypt.Net;
    using Core.Services;
    using HRP.Core.Criteria;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Account;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Resources;
    using Services;

    public class AuthController : BaseController
    {
        private readonly IUserService userService;

        public AuthController(
            IUserService userService,
            ISessionService sessionService,
            IBaseService baseService) : base(baseService, sessionService)
        {
            this.userService = userService;
        }

        public IActionResult Login()
        {
            sessionService.ClearAll();
            var viewModel = new LoginViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel)
        {
            string errorMsg = string.Empty;

            if (ModelState.IsValid)
            {
                var user = userService.GetUserByUsername(viewModel.Username);
                var password = Utilities.DecryptStringAES(viewModel.Password);

                if (user != null && userService.IsCorrectPassword(password, user.Password) && user.IsActive)
                {
                    return AfterLoginSuccessful(user);
                }

                errorMsg = (user != null && userService.IsCorrectPassword(password, user.Password) && !user.IsActive) ? Message.AccountInactive : Message.LoginInvalid;
            }

            ModelState.AddModelError(string.Empty, errorMsg);

            return View(viewModel);
        }

        private IActionResult AfterLoginSuccessful(User user)
        {
            if (!user.IsOTPRegistered)
            {
                var otpRegisterToken = userService.GenerateTokenOTPRegister(user.Username);

                userService.SendMailToRegisterOTP(otpRegisterToken);

                return RedirectToAction("RegisterOTPNotification", "Account", new { name = user.Username });
            }

            sessionService.Username = user.Username;

            return RedirectToAction("VerifyOTP", "Account");
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string username)
        {
            var user = userService.GetUserByUsername(username);

            if (user != null)
            {
                userService.ResetUserPassword(username);

                return RedirectToAction(nameof(ConfirmationEmailSent));
            }

            ModelState.AddModelError(string.Empty, Message.ForgotPasswordInvalidUser);

            return View();
        }

        public IActionResult ConfirmationEmailSent()
        {
            return View();
        }

        public IActionResult ResetPassword(string token)
        {
            var user = baseService.GetDataEntity<User>(new GetUserByTokenResetPasswordCriteria { TokenResetPassword = token });

            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View();
        }

        public IActionResult ResetPasswordSuccessfully()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string token, ResetPasswordViewModel model)
        {
            var user = baseService.GetDataEntity<User>(new GetUserByTokenResetPasswordCriteria { TokenResetPassword = token });

            if (model != null && ModelState.IsValid && user != null)
            {
                var criteria = new UpdateUserPasswordCriteria
                {
                    UserId = user.UserId,
                    Password = BCrypt.HashPassword(model.Password)
                };

                var result = baseService.ExecuteNonQuery(criteria);
                if (result.IsSuccessful)
                {
                    return RedirectToAction(nameof(ResetPasswordSuccessfully));
                }

                ModelState.AddModelError(string.Empty, result.Message);
            }

            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await DoSignOut();

            return RedirectToAction(nameof(Login));
        }

        private async Task DoSignOut()
        {
            var criteria = new LogOutUserCriteria
            {
                OnUserId = sessionService.OnUserId,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                Platform = HttpContext.Request.Headers["User-Agent"].ToString()
            };
            await baseService.ExecuteNonQueryAsync(criteria);

            sessionService.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}