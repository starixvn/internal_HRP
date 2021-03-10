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
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Account;
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
            (var rijndaelKeyPair, var rijndaelIVPair) = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[rijndaelKeyPair.Key] = rijndaelKeyPair.Value;
            TempData[rijndaelIVPair.Key] = rijndaelIVPair.Value;

            var viewModel = new LoginViewModel
            {
                RijndaelKey = rijndaelKeyPair,
                RijndaelIV = rijndaelIVPair
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel viewModel)
        {
            string errorMsg = string.Empty;

            if (ModelState.IsValid)
            {
                var rijndaelKey = (string)TempData[viewModel.RijndaelKey.Key];
                var rijndaelIV = (string)TempData[viewModel.RijndaelIV.Key];
                User user = await userService.GetUserByUsernameAsync(viewModel.Username);
                var password = rijndaelService.Decrypt(viewModel.Password, rijndaelKey, rijndaelIV);

                if (user != null && userService.IsCorrectPassword(password, user.Password) && user.IsActive)
                {
                    return await AfterLoginSuccessful(user);
                }

                errorMsg = (user != null && userService.IsCorrectPassword(password, user.Password) && !user.IsActive)
                    ? Message.AccountInactive
                    : Message.LoginInvalid;
            }

            ModelState.AddModelError(string.Empty, errorMsg);
            (var rijndaelKeyPair, var rijndaelIVPair) = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[rijndaelKeyPair.Key] = rijndaelKeyPair.Value;
            TempData[rijndaelIVPair.Key] = rijndaelIVPair.Value;
            viewModel.RijndaelKey = rijndaelKeyPair;
            viewModel.RijndaelIV = rijndaelIVPair;

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

            (var rijndaelKeyPair, var rijndaelIVPair) = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[rijndaelKeyPair.Key] = rijndaelKeyPair.Value;
            TempData[rijndaelIVPair.Key] = rijndaelIVPair.Value;

            var viewModel = new ResetPasswordViewModel
            {
                RijndaelKey = rijndaelKeyPair,
                RijndaelIV = rijndaelIVPair
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string token, ResetPasswordViewModel model)
        {
            var rijndaelKey = (string)TempData[model.RijndaelKey.Key];
            var rijndaelIV = (string)TempData[model.RijndaelIV.Key];
            var user = await baseService.GetDataEntityAsync<User>(new GetUserByTokenResetPasswordCriteria { TokenResetPassword = token });

            model.Password = rijndaelService.Decrypt(model.Password, rijndaelKey, rijndaelIV);
            model.PasswordConfirm = rijndaelService.Decrypt(model.PasswordConfirm, rijndaelKey, rijndaelIV);

            if (ModelState.IsValid && user != null)
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

            (var rijndaelKeyPair, var rijndaelIVPair) = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[rijndaelKeyPair.Key] = rijndaelKeyPair.Value;
            TempData[rijndaelIVPair.Key] = rijndaelIVPair.Value;

            model.RijndaelKey = rijndaelKeyPair;
            model.RijndaelIV = rijndaelIVPair;

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