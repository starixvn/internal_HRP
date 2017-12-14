namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using BCrypt.Net;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Filters;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Criteria.User;
    using HRP.Models;
    using HRP.Models.Account;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Resources;
    using Services;

    [HRPAuthorize]
    public class AccountController : Controller
    {
        private readonly IBaseService _baseService;
        private readonly ISessionService _sessionService;
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

        public IActionResult Index(AccountTab tab = AccountTab.Profile)
        {
            var user = _userService.GetUserByUsername(_sessionService.Username);

            var userProfile = new UserProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone
            };

            AccountViewModel accountViewModel = new AccountViewModel
            {
                ActivePanel = tab,
                IsOTPRegistered = user.IsOTPRegistered,
                UserProfileViewModel = userProfile
            };

            return View(accountViewModel);
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

                if (result.IsSuccessful)
                {
                    return Json(new { code = result.Code, message = Message.PasswordChangeSuccessful });
                }
            }

            return Json(new { code = -1, message = Message.UpdateFail });
        }

        [HttpPost]
        public IActionResult UpdateProfile(UserProfileViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateUserProfileCriteria
                {
                    UserId = _sessionService.UserId,
                    FullName = !string.IsNullOrEmpty(viewModel.FullName) ? viewModel.FullName : string.Empty,
                    Phone = !string.IsNullOrEmpty(viewModel.Phone) ? viewModel.Phone : string.Empty,
                    Email = viewModel.Email
                };

                var result = _baseService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    return Json(new { code = result.Code, message = Message.UpdateSuccess });
                }
            }
            return Json(new { code = -1, message = Message.UpdateFail });
        }

        [AllowAnonymous]
        public IActionResult RegisterOTPNotification()
        {
            return View();
        }

        public IActionResult ResetOtpDevice()
        {
            // step 1: Reset IsOTPRegistered
            _userService.UpdateOTPStatus(_sessionService.UserId, false);

            // step 2: Reset OTP token
            var otpRegisterToken = _userService.GenerateTokenOTPRegister(_sessionService.Username);

            // step 3: Sign user out
            ////await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).ConfigureAwait(false);

            // step 4: Show SetupOtp screen
            return RedirectToAction(
                "SetupOtp",
                "Account",
                new { token = otpRegisterToken });
        }

        [AllowAnonymous]
        public IActionResult SetupOtp(string token)
        {
            var user = _baseService.GetDataEntity<User>(new GetUserByTokenOTPRegisterCriteria
            {
                TokenOTPRegister = token
            });

            if (string.IsNullOrEmpty(token) || user == null)
            {
                return BadRequest();
            }

            var userProfile = new UserProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone
            };

            var setupOtpViewModel = new SetupOtpViewModel
            {
                TextCode = GetOtpSetupCode(user.Username),
                Username = user.Username
            };

            AccountViewModel accountViewModel = new AccountViewModel
            {
                ActivePanel = AccountTab.Otp,
                IsOTPRegistered = false,
                SetupOtpViewModel = setupOtpViewModel,
                UserProfileViewModel = userProfile
            };

            return User.Identity.IsAuthenticated ? View("Index", accountViewModel) : View("SetupOtp", accountViewModel.SetupOtpViewModel);
        }

        [AllowAnonymous]
        public IActionResult VerifyOTP()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

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
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            var user = _userService.GetUserByUsername(_sessionService.Username);

            if (!ModelState.IsValid || user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var isOTPValid = _twoFactorAuthService.ValidateOTPCode(user.SecretKey, viewModel.OTPCode);

            var isSkipOTP = viewModel.IsByPassTwoFactor;

            if (isOTPValid || isSkipOTP)
            {
                return await SignUserIn(user);
            }

            ViewBag.ErrorMessage = Message.InvalidOTP;

            return RedirectToAction(nameof(VerifyOTP));
        }

        private async Task<IActionResult> SignUserIn(User user)
        {
            _sessionService.UserId = user.UserId;
            _sessionService.Username = user.Username;

            await PopulateMenusAndFunctions(user);

            ClaimsPrincipal principal = InitUserClaimsPrinciple(user);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal)
                .ConfigureAwait(false);

            _baseService.ExecuteNonQuery(new UpdateUserLastLoginDateCriteria { UserId = _sessionService.UserId });

            return RedirectToAction("Index", "Home");
        }

        private async Task PopulateMenusAndFunctions(User user)
        {
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
        }

        private static ClaimsPrincipal InitUserClaimsPrinciple(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.GivenName, user.FullName),
                new Claim(Constants.IsSystemAdmin, user.IsSystemAdmin.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return new ClaimsPrincipal(claimsIdentity);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult VerifySetupOTP(SetupOtpViewModel viewModel)
        {
            var user = _userService.GetUserByUsername(viewModel.Username);

            if (!ModelState.IsValid || user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var isOTPValid = _twoFactorAuthService.ValidateOTPCode(user.SecretKey, viewModel.Otp);

            if (!isOTPValid)
            {
                return BadRequest();
            }

            _userService.UpdateOTPStatus(user.UserId, true);
            _userService.ResetTokenOTPRegister(user.Username);
            _sessionService.Username = user.Username;
            return new OkResult();
        }

        private string GetOtpSetupCode(string username)
        {
            var secretKey = Utilities.GenerateSecretKey();
            _userService.UpdateSecretKey(username, secretKey);

            return _twoFactorAuthService.SetupGoogleAuthInfo(_sessionService.Username, secretKey);
        }
    }
}