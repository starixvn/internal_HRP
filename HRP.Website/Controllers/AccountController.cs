namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using BCrypt.Net;
    using Core.Filters;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Criteria.User;
    using HRP.Models;
    using HRP.Models.Account;
    using HRP.Models.UserProfile;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Resources;
    using Services;

    [HRPAuthorize]
    public class AccountController : Controller
    {
        private readonly ISessionService _sessionService;

        private readonly IBaseService _baseService;

        private readonly ITwoFactorAuthenticatorService _twoFactorAuthService;

        private readonly IUserService _userService;

        public AccountController(
            ISessionService sessionService,
            IBaseService baseService,
            ITwoFactorAuthenticatorService twoFactorAuthService,
            IUserService userService)
        {
            _sessionService = sessionService;
            _baseService = baseService;
            _twoFactorAuthService = twoFactorAuthService;
            _userService = userService;
        }

        //[AllowAnonymous]
        //public IActionResult Index(AccountViewModel viewModel)
        //{
        //    var user = _userService.GetUserByUsername(_sessionService.Username);
        //    if (viewModel.ActivePanel == null)
        //    {
        //    }

        //    return View(viewModel);
        //}
                [AllowAnonymous]
        public IActionResult SetupOtp(string token, string username)
        {
            //#if DEBUG
            //            var testUser = _userService.GetUserByUsername(username);
            //            var secretKey = testUser.SecretKey;
            //#else
            var secretKey = Utilities.GenerateSecretKey();
            _userService.UpdateSecretKey(username, secretKey);
            //#endif
            //TODO: remove after test
            ////var secretKey = _userService.GetUserByUsername(username).SecretKey;

            var setupCode = _twoFactorAuthService.SetupGoogleAuthInfo(_sessionService.Username, secretKey);

            if (!string.IsNullOrEmpty(token) && !IsAuthenticated())
            {
                var user = _baseService.GetDataEntity<User>(new GetUserByTokenOTPRegisterCriteria { TokenOTPRegister = token });
                if (user == null)
                {
                    return BadRequest();
                }
            }

            AccountViewModel accountViewModel = new AccountViewModel
            {
                ActivePanel = "otp",
                IsOTPRegistered = false,
                SetupOtpViewModel = new SetupOtpViewModel
                {
                    TextCode = setupCode.ManualEntryKey,
                    Username = username,
                    SystemName = "HRP"
                }
            };

            return View("Index", accountViewModel);
        }

        [AllowAnonymous]
        public IActionResult RegisterOTPNotification()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult VerifySetupOTP(SetupOtpViewModel viewModel)
        {
            var user = _userService.GetUserByUsername(viewModel.Username);

            if (!ModelState.IsValid || user == null)
            {
                //TODO: Re-enable when test complete
                return RedirectToAction("Login", "Auth");
            }

            var isOTPValid = _twoFactorAuthService.ValidateOTPCode(_sessionService.Username, user.SecretKey, viewModel.Otp);

            if (isOTPValid)
            {
                var result = _userService.UpdateOTPStatus(user.UserId, true);

                if (result)
                {
                    _sessionService.Username = user.Username;
                    return new OkResult();
                }
            }

            //TODO: show error message
            ViewBag.ErrorMessage = Message.InvalidOTP;

            return BadRequest();
        }

        [AllowAnonymous]
        public IActionResult VerifyOTP()
        {
            if (string.IsNullOrEmpty(_sessionService.Username))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(new TwoFactorAuthenticatorViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOTP(TwoFactorAuthenticatorViewModel viewModel)
        {
            var user = _userService.GetUserByUsername(_sessionService.Username);

            if (!ModelState.IsValid || user == null)
            {
                return RedirectToAction("Login", "Auth");
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

        private bool IsAuthenticated()
        {
            return _sessionService.UserId > 0 && !string.IsNullOrEmpty(_sessionService.Username);
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel viewlModel)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetUserByUsername(_sessionService.Username);
                var isValidOldPassword = BCrypt.Verify(viewlModel.OldPassword, user.Password);

                if (!isValidOldPassword)
                {
                    return Json(new { code = -2, message = Message.OldPasswordIncorrect });
                }

                var criteria = new UpdateUserPasswordCriteria
                {
                    UserId = _sessionService.UserId,
                    Password = BCrypt.HashPassword(viewlModel.Password)
                };

                var result = _baseService.ExecuteNonQuery(criteria);

                return Json(new { code = result.Code, message = Message.PasswordChangeSuccessful });
            }

            return Json(new { code = -1, message = "Error" });
        }
    }
}