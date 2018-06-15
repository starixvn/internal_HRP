namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using BCrypt.Net;
    using Core.Criteria;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Models.Account;
    using Models;
    using Resources;
    using Services;

    public class AuthController : Controller
    {
        private readonly IBaseService baseService;
        private readonly IRijndaelService rijndaelService;
        private readonly ISessionService sessionService;
        private readonly IUserService userService;

        public AuthController(
            IUserService userService,
            ISessionService sessionService,
            IBaseService baseService,
            IRijndaelService rijndaelService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.userService = userService;
            this.rijndaelService = rijndaelService;
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
        public async Task<IActionResult> ForgotPassword(string username)
        {
            User user = await userService.GetUserByUsernameAsync(username);

            if (user != null)
            {
                await userService.ResetUserPasswordAsync(username);

                return RedirectToAction(nameof(ConfirmationEmailSent));
            }

            ModelState.AddModelError(string.Empty, Message.ForgotPasswordInvalidUser);

            return View();
        }

        public IActionResult Login()
        {
            Tuple<string, string> tuple = rijndaelService.GenerateKey();
            sessionService.RijndaelKey = tuple.Item1;
            sessionService.RijndaelIV = tuple.Item2;
            var viewModel = new LoginViewModel
            {
                RijndaelIV = sessionService.RijndaelIV,
                RijndaelKey = sessionService.RijndaelKey
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            string errorMsg = string.Empty;

            if (ModelState.IsValid)
            {
                User user = await userService.GetUserByUsernameAsync(viewModel.Username);
                var password = rijndaelService.Decrypt(viewModel.Password, sessionService.RijndaelKey, sessionService.RijndaelIV);

                if (user != null && userService.IsCorrectPassword(password, user.Password) && user.IsActive)
                {
                    sessionService.RijndaelKey = string.Empty;
                    sessionService.RijndaelIV = string.Empty;
                    return await AfterLoginSuccessful(user);
                }

                errorMsg = (user != null && userService.IsCorrectPassword(password, user.Password) && !user.IsActive)
                    ? Message.AccountInactive
                    : Message.LoginInvalid;
            }

            ModelState.AddModelError(string.Empty, errorMsg);
            Tuple<string, string> tuple = rijndaelService.GenerateKey();
            sessionService.RijndaelKey = tuple.Item1;
            sessionService.RijndaelIV = tuple.Item2;
            viewModel.RijndaelIV = sessionService.RijndaelIV;
            viewModel.RijndaelKey = sessionService.RijndaelKey;

            return View(viewModel);
        }

        public async Task<IActionResult> Logout()
        {
            await DoSignOut();

            return RedirectToAction(nameof(Login));
        }

        public async Task<IActionResult> ResetPassword(string token)
        {
            var user = await baseService.GetDataEntityAsync<User>(new GetUserByTokenResetPasswordCriteria { TokenResetPassword = token });

            if (user == null || user.UserId <= 0)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token, ResetPasswordViewModel model)
        {
            var user = await baseService.GetDataEntityAsync<User>(new GetUserByTokenResetPasswordCriteria { TokenResetPassword = token });

            if (model != null && ModelState.IsValid && user != null)
            {
                var criteria = new UpdateUserPasswordCriteria
                {
                    UserId = user.UserId,
                    Password = BCrypt.HashPassword(model.Password)
                };

                CommandResult result = await baseService.ExecuteNonQueryAsync(criteria);
                if (result.IsSuccessful)
                {
                    return RedirectToAction(nameof(ResetPasswordSuccessfully));
                }

                ModelState.AddModelError(string.Empty, result.Message);
            }

            return View(model);
        }

        public IActionResult ResetPasswordSuccessfully()
        {
            return View();
        }

        private async Task<IActionResult> AfterLoginSuccessful(User user)
        {
            if (!user.IsOTPRegistered)
            {
                string otpRegisterToken = await userService.GenerateTokenOTPRegisterAsync(user.Username);

                await userService.SendMailToRegisterOTPAsync(otpRegisterToken);

                return RedirectToAction("RegisterOTPNotification", "Account", new { name = user.Username });
            }

            sessionService.Username = user.Username;

            return RedirectToAction("VerifyOTP", "Account");
        }

        private void ClearCookies()
        {
            HttpContext.Response.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache");
            HttpContext.Response.Headers.Add("Expires", "0");

            IEnumerable<string> cookies = HttpContext.Request.Cookies.Keys;
            foreach (string cookie in cookies)
            {
                HttpContext.Response.Cookies.Delete(cookie);
            }
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

            ClearCookies();
            sessionService.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}