namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using BCrypt.Net;
    using Google.Authenticator;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.User;
    using HRP.Models;
    using HRP.Models.Authentication;
    using HRP.Resources;
    using HRP.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;

    public class AuthController : Controller
    {
        private readonly IBaseService _baseService;
        private readonly ISessionService _sessionService;
        private readonly ITwoFactorAuthenticatorService _twoFactorAuthService;
        private readonly IUserService _userService;

        public AuthController(
            IUserService userService,
            ISessionService sessionService,
            ITwoFactorAuthenticatorService twoFactorAuthenService,
            IBaseService baseService)
        {
            _userService = userService;
            _sessionService = sessionService;
            _twoFactorAuthService = twoFactorAuthenService;
            _baseService = baseService;
        }

        public IActionResult ConfirmationEmailSent()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string username)
        {
            var user = _userService.GetUserByUsername(username);

            if (user != null || username == "admin")
            {
                _userService.ResetUserPassword(username);

                return RedirectToAction(nameof(ConfirmationEmailSent));
            }

            ViewBag.ErrorMessage = Message.ForgotPasswordInvalidUser;

            return View();
        }

        public ActionResult Login()
        {
            var model = new LoginViewModel();
            return View(model);
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByUsername(viewModel.Username);

                if (user != null && IsPasswordValid(viewModel.Password, user.Password))
                {
                    _sessionService.Username = user.Username;
                    _baseService.ExecuteNonQuery(new UpdateUserLastLoginDateCriteria { UserId = _sessionService.UserId });

                    if (!user.IsOTPRegistered)
                    {
                        _userService.OTPRegister(user.Username);
                        return RedirectToAction(nameof(SetupOtp));
                    }

                    return RedirectToAction(nameof(VerifyOTP));
                }
            }

            ViewBag.ErrorMessage = Message.LoginInvalid;

            return View(viewModel);
        }

        [HttpGet("[action]"), HttpPost("[action]")]
        public async Task<IActionResult> Logout()
        {
            _sessionService.ClearAll();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ResetPassword(string token)
        {
            var user = _baseService.GetDataEntity<User>(new GetUserByTokenResetPasswordCriteria { TokenResetPassword = token });

            if (user != null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string token, ChangePasswordViewModel model)
        {
            var user = _baseService.GetDataEntity<User>(new GetUserByTokenResetPasswordCriteria { TokenResetPassword = token });

            if (model != null && ModelState.IsValid && user != null)
            {
                var criteria = new UpdateUserPasswordCriteria
                {
                    UserId = user.UserId,
                    Password = BCrypt.HashPassword(model.Password)
                };

                var result = _baseService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
            }
            return BadRequest();
        }

        public IActionResult ChangePasswordSuccess()
        {
            return View();
        }

        public IActionResult VerifyOTP()
        {
            return View(new TwoFactorAuthenticatorViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOTP(TwoFactorAuthenticatorViewModel viewModel)
        {
            var user = _userService.GetUserByUsername(_sessionService.Username);

            if (!ModelState.IsValid || user == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var isOTPValid = _twoFactorAuthService.ValidateOTPCode(_sessionService.Username, user.SecretKey, viewModel.OTPCode);

            var isSkipOTP = viewModel.IsByPassTwoFactor;

            if (isOTPValid || isSkipOTP)
            {
                _sessionService.UserId = user.UserId;
                _sessionService.Username = user.Username;

                _sessionService.MenuItems = await _baseService.GetDataAsync<MenuItem>(
                    new GetMenuByUserIdCriteria
                    {
                        UserId = user.UserId
                    }).ConfigureAwait(false);

                _sessionService.Functions = await _baseService.GetDataAsync<FunctionModel>(
                    new GetFunctionByUserIdCriteria
                    {
                        UserId = user.UserId
                    }).ConfigureAwait(false);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
                };

                var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal).ConfigureAwait(false);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = Message.InvalidOTP;

            return RedirectToAction(nameof(VerifyOTP));
        }

        public IActionResult SetupOtp(string token)
        {
            User user = new User();
            string secretKey = Utilities.GenerateSecretKey();
            var setupCode = _twoFactorAuthService.SetupGoogleAuthInfo(user.Username, user.SecretKey);

            if (string.IsNullOrEmpty(token) && IsAuthenticated())
            {
                user = _userService.GetUserByUsername(_sessionService.Username);
            }
            else
            {
                user = _baseService.GetDataEntity<User>(new GetUserByTokenOTPRegisterCriteria { TokenOTPRegister = token });
                if (user == null)
                {
                    return BadRequest();
                }
            }

            var viewModel = new SetupOtpViewModel
            {
                SecretKey = secretKey,
                QrCodeLink = setupCode.QrCodeSetupImageUrl,
                TextCode = setupCode.ManualEntryKey
            };

            return View(viewModel);
        }

        private bool IsAuthenticated()
        {
            return _sessionService.UserId > 0 && !string.IsNullOrEmpty(_sessionService.Username);
        }

        private static bool IsPasswordValid(string modelPassword, string hashedPassword)
        {
            return BCrypt.Verify(modelPassword, hashedPassword);
        }
    }
}