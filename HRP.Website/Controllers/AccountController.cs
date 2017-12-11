namespace HRP.Website.Controllers
{
    using BCrypt.Net;
    using Core.Filters;
    using Core.Services;
    using Criteria.User;
    using HRP.Models;
    using HRP.Models.Account;
    using HRP.Models.UserProfile;
    using HRP.Resources;
    using HRP.Services;
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

        [AllowAnonymous]
        public IActionResult Index(AccountViewModel viewModel)
        {
            return View(viewModel);
        }

        [AllowAnonymous]
        public IActionResult SetupOtp(string token, string username)
        {
#if DEBUG
            var testUser = _userService.GetUserByUsername(username);
            var secretKey = testUser.SecretKey;
#else
            var secretKey = Utilities.GenerateSecretKey();
            _userService.UpdateSecretKey(username, secretKey);
#endif

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
                    SecretKey = secretKey,
                    QrCodeLink = setupCode.QrCodeSetupImageUrl,
                    TextCode = setupCode.ManualEntryKey,
                    Username = username
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
        public IActionResult VerifyOTP(SetupOtpViewModel viewModel)
        {
            var user = _userService.GetUserByUsername(viewModel.Username);

            if (!ModelState.IsValid || user == null)
            {
                //TODO: Re-enable when test complete
                return RedirectToAction("Login", "Auth");
            }

            var isOTPValid = _twoFactorAuthService.ValidateOTPCode(_sessionService.Username, user.SecretKey, viewModel.Otp);

#if DEBUG
            isOTPValid = true;
#endif

            if (isOTPValid)
            {
                var result = _userService.UpdateOTPStatus(user.UserId, true);

                if (result)
                {
                    return new OkResult();
                }
            }

            //TODO: show error message
            ViewBag.ErrorMessage = Message.InvalidOTP;

            return BadRequest();
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

                if (BCrypt.Verify(viewlModel.OldPassword, user.Password))
                {
                    var criteria = new UpdateUserPasswordCriteria
                    {
                        UserId = _sessionService.UserId,
                        Password = BCrypt.HashPassword(viewlModel.Password)
                    };

                    var result = _baseService.ExecuteNonQuery(criteria);

                    if (result.IsSuccessful)
                    {
                        ViewBag.Message = Message.PasswordChangeSuccessful;
                    }
                }
                else
                {
                    ViewBag.Message = Message.OldPasswordIncorrect;
                }
            }

            return RedirectToAction("Index", "Account", "password");
        }
    }
}