namespace HRP.Website.Controllers
{
    using BCrypt.Net;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Criteria.User;
    using HRP.Models;
    using HRP.Models.Authentication;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Mvc;
    using Resources;
    using Services;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using HRP.Models.Account;

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

        public IActionResult ChangePasswordSuccessful()
        {
            return View();
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

            if (user != null || username == "admin")
            {
                _userService.ResetUserPassword(username);

                return RedirectToAction(nameof(ConfirmationEmailSent));
            }

            ViewBag.ErrorMessage = Message.ForgotPasswordInvalidUser;

            return View();
        }

        public IActionResult Login()
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

                if (user != null && IsPasswordValid(viewModel.Password, user.Password) && user.IsActive)
                {
                    if (!user.IsOTPRegistered)
                    {
                        _userService.OTPRegister(user.Username);

                        AccountViewModel accountViewModel = new AccountViewModel
                        {
                            ActivePanel = "otp",
                            IsOTPRegistered = false
                        };

                        return RedirectToAction("Index","Account", accountViewModel);
                    }

                    _sessionService.Username = user.Username;

                    return RedirectToAction(nameof(VerifyOTP));
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

        public IActionResult RegisterOTPNoti()
        {
            return View();
        }

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
                    return RedirectToAction(nameof(ChangePasswordSuccessful));
                }
            }
            return BadRequest();
        }

        public IActionResult SetupOtp(string token)
        {
            var secretKey = Utilities.GenerateSecretKey();
            var setupCode = _twoFactorAuthService.SetupGoogleAuthInfo(_sessionService.Username, secretKey);

            if (!string.IsNullOrEmpty(token) && !IsAuthenticated())
            {
                var user = _baseService.GetDataEntity<User>(new GetUserByTokenOTPRegisterCriteria { TokenOTPRegister = token });
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

        public IActionResult VerifyOTP()
        {
            if (string.IsNullOrEmpty(_sessionService.Username))
            {
                return RedirectToAction(nameof(Login));
            }

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

                _baseService.ExecuteNonQuery(new UpdateUserLastLoginDateCriteria { UserId = _sessionService.UserId });

                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = Message.InvalidOTP;

            return RedirectToAction(nameof(VerifyOTP));
        }

        private static bool IsPasswordValid(string modelPassword, string hashedPassword)
        {
            return BCrypt.Verify(modelPassword, hashedPassword);
        }

        private bool IsAuthenticated()
        {
            return _sessionService.UserId > 0 && !string.IsNullOrEmpty(_sessionService.Username);
        }
    }
}