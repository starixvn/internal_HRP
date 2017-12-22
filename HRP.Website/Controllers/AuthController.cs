namespace HRP.Website.Controllers
{
    using System.Threading.Tasks;
    using BCrypt.Net;
    using Core.Services;
    using HRP.Core.Criteria;
    using HRP.Core.Model;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Account;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Resources;
    using Services;

    public class AuthController : Controller
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly IUserService userService;

        public AuthController(
            IUserService userService,
            ISessionService sessionService,
            IBaseService baseService)
        {
            this.userService = userService;
            this.sessionService = sessionService;
            this.baseService = baseService;
        }

        public IActionResult ConfirmationEmailSent()
        {
            return View();
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

            ViewBag.ErrorMessage = Message.ForgotPasswordInvalidUser;

            return View();
        }

        public IActionResult Login()
        {
            sessionService.ClearAll();
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel)
        {
            string errorMsg = string.Empty;

            if (ModelState.IsValid)
            {
                var user = userService.GetUserByUsername(viewModel.Username);
                var password = Utilities.DecryptStringAES(viewModel.Password);

                if (user != null && IsPasswordValid(password, user.Password) && user.IsActive)
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

                errorMsg = (user != null && IsPasswordValid(password, user.Password) && !user.IsActive) ? Message.AccountInactive : Message.LoginInvalid;
            }

            ViewBag.ErrorMessage = errorMsg;
            return View(viewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await DoSignOut();

            return RedirectToAction(nameof(Login));
        }

        public IActionResult ResetPassword(string token)
        {
            var user = baseService.GetDataEntity<User>(new GetUserByTokenResetPasswordCriteria { TokenResetPassword = token });

            if (user == null || user.UserId <= 0)
            {
                return NotFound();
            }

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

                CommandResult result = baseService.ExecuteNonQuery(criteria);
                if (result.IsSuccessful)
                {
                    return RedirectToAction(nameof(ResetPasswordSuccessfully));
                }
            }

            ViewBag.ErrorMessage = Message.UpdateFail;

            return View(model);
        }

        public IActionResult ResetPasswordSuccessfully()
        {
            return View();
        }

        private static bool IsPasswordValid(string modelPassword, string hashedPassword)
        {
            return BCrypt.Verify(modelPassword, hashedPassword);
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