namespace HRP.Website.Controllers
{
    using System.Threading.Tasks;
    using BCrypt.Net;
    using Core.Services;
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
        private readonly IBaseService _baseService;
        private readonly ISessionService _sessionService;
        private readonly IUserService _userService;

        public AuthController(
            IUserService userService,
            ISessionService sessionService,
            IBaseService baseService)
        {
            _userService = userService;
            _sessionService = sessionService;
            _baseService = baseService;
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
            var user = _userService.GetUserByUsername(username);

            if (user != null)
            {
                _userService.ResetUserPassword(username);

                return RedirectToAction(nameof(ConfirmationEmailSent));
            }

            ViewBag.ErrorMessage = Message.ForgotPasswordInvalidUser;

            return View();
        }

        public IActionResult Login()
        {
            _sessionService.ClearAll();
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByUsername(viewModel.Username);
                var password = Utilities.DecryptStringAES(viewModel.Password);

                if (user != null && IsPasswordValid(password, user.Password) && user.IsActive)
                {
                    if (!user.IsOTPRegistered)
                    {
                        var otpRegisterToken = _userService.GenerateTokenOTPRegister(user.Username);

                        _userService.SendMailToRegisterOTP(otpRegisterToken);

                        return RedirectToAction("RegisterOTPNotification", "Account", new { name = user.FullName });
                    }

                    _sessionService.Username = user.Username;

                    return RedirectToAction("VerifyOTP", "Account");
                }
            }

            ViewBag.ErrorMessage = Message.LoginInvalid;

            return View(viewModel);
        }

        public async Task<IActionResult> Logout()
        {
            _sessionService.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            return RedirectToAction(nameof(Login));
        }

        public IActionResult ResetPassword(string token)
        {
            var user = _baseService.GetDataEntity<User>(new GetUserByTokenResetPasswordCriteria { TokenResetPassword = token });

            if (user == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string token, ResetPasswordViewModel model)
        {
            var user = _baseService.GetDataEntity<User>(new GetUserByTokenResetPasswordCriteria { TokenResetPassword = token });

            if (model != null && ModelState.IsValid && user != null)
            {
                var criteria = new UpdateUserPasswordCriteria
                {
                    UserId = user.UserId,
                    Password = BCrypt.HashPassword(model.Password)
                };

                CommandResult result = _baseService.ExecuteNonQuery(criteria);
                ViewBag.IsSuccessful = result.IsSuccessful;
            }

            return View(model);
        }

        private static bool IsPasswordValid(string modelPassword, string hashedPassword)
        {
            return BCrypt.Verify(modelPassword, hashedPassword);
        }
    }
}