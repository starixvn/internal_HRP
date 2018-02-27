namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
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
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Account;
    using Resources;
    using Services;
    using Services.Interfaces;

    public class AccountController : BaseController
    {
        private readonly ITwoFactorAuthenticatorService twoFactorAuthService;
        private readonly IUserService userService;

        public AccountController(
            ISessionService sessionService,
            IBaseService baseService,
            ITwoFactorAuthenticatorService twoFactorAuthService,
            IUserService userService) : base(baseService, sessionService)
        {
            this.twoFactorAuthService = twoFactorAuthService;
            this.userService = userService;
        }

        [HRPAuthorize]
        public IActionResult Index(AccountTab tab = AccountTab.Profile)
        {
            var user = userService.GetUserByUsername(sessionService.Username);

            var userProfile = new UserProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone
            };

            var accountViewModel = new AccountViewModel
            {
                ActivePanel = tab,
                IsOTPRegistered = user.IsOTPRegistered,
                UserProfileViewModel = userProfile
            };

            ViewData["Username"] = sessionService.Username;

            return View(accountViewModel);
        }

        [HRPAuthorize]
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (!IsCorrectOldPassword(viewModel.OldPassword))
                {
                    return Json(new { code = -2, message = Message.OldPasswordIncorrect });
                }

                if (IsSuccessUpdatePassword(viewModel.Password))
                {
                    return Json(new { code = 0, message = Message.PasswordChangeSuccessful });
                }
            }

            return Json(new { code = -1, message = Message.UpdateFail });
        }

        private bool IsCorrectOldPassword(string oldPassword)
        {
            var user = userService.GetUserByUsername(sessionService.Username);
            var isCorrectOldPassword = userService.IsCorrectPassword(oldPassword, user.Password);
            return isCorrectOldPassword;
        }

        private bool IsSuccessUpdatePassword(string newPassword)
        {
            var criteria = new UpdateUserPasswordCriteria
            {
                UserId = sessionService.UserId,
                Password = BCrypt.HashPassword(newPassword)
            };

            var result = baseService.ExecuteNonQuery(criteria);

            return result.IsSuccessful;
        }

        [AllowAnonymous]
        public IActionResult RegisterOTPNotification(string name)
        {
            return View("RegisterOTPNotification", name);
        }

        [HRPAuthorize]
        public IActionResult ResetOtpDevice()
        {
            // step 1: Reset IsOTPRegistered
            userService.UpdateOTPStatus(sessionService.UserId, false);

            // step 2: Reset OTP token
            var otpRegisterToken = userService.GenerateTokenOTPRegister(sessionService.Username);

            // step 3: Show SetupOtp screen
            return RedirectToAction("SetupOtpAfterReset", "Account", new { token = otpRegisterToken });
        }

        [AllowAnonymous]
        public IActionResult SetupOtp(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            User user = userService.GetUserByTokenOTPRegister(token);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var setupOtpViewModel = new SetupOtpViewModel
            {
                TextCode = GetOtpSetupCode(user.Username),
                Username = user.Username
            };

            return View("SetupOtp", setupOtpViewModel);
        }

        [HRPAuthorize]
        public IActionResult SetupOtpAfterReset(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Auth");
            }

            User user = userService.GetUserByTokenOTPRegister(token);
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
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

            var accountViewModel = new AccountViewModel
            {
                ActivePanel = AccountTab.Otp,
                IsOTPRegistered = false,
                SetupOtpViewModel = setupOtpViewModel,
                ChangePasswordViewModel = new ChangePasswordViewModel(),
                UserProfileViewModel = userProfile
            };

            return View("Index", accountViewModel);
        }

        [HRPAuthorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserProfileViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateUserProfileCriteria
                {
                    UserId = sessionService.UserId,
                    FullName = !string.IsNullOrEmpty(viewModel.FullName) ? viewModel.FullName : string.Empty,
                    Phone = !string.IsNullOrEmpty(viewModel.Phone) ? viewModel.Phone : string.Empty,
                    Email = viewModel.Email
                };

                CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria);

                return Json(commandResult);
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        [AllowAnonymous]
        public IActionResult VerifyOTP()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", new { tab = AccountTab.Otp });
            }

            if (string.IsNullOrEmpty(sessionService.Username))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View("VerifyOTP", new TwoFactorAuthenticatorViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOTP(TwoFactorAuthenticatorViewModel viewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", new { tab = AccountTab.Otp });
            }

            User user = userService.GetUserByUsername(sessionService.Username);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var isOTPValid = twoFactorAuthService.ValidateOTPCode(user.SecretKey, viewModel.OTPCode);

            // TODO: Remove IsByPassTwoFactor
            if (ModelState.IsValid && (isOTPValid || viewModel.IsByPassTwoFactor))
            {
                await SignUserIn(user);
                return RedirectToAction("Index", "Home");
            }

            ViewData["ErrorMessage"] = Message.InvalidOTP;

            return View("VerifyOTP");
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult VerifySetupOTP(SetupOtpViewModel viewModel)
        {
            User user = userService.GetUserByUsername(viewModel.Username);

            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var isOTPValid = twoFactorAuthService.ValidateOTPCode(user.SecretKey, viewModel.Otp);

            if (!ModelState.IsValid || !isOTPValid)
            {
                return Forbid();
            }

            userService.UpdateOTPStatus(user.UserId, true);
            userService.ResetTokenOTPRegister(user.Username);
            sessionService.Username = user.Username;
            return new OkResult();
        }

        private static ClaimsPrincipal InitUserClaimsPrinciple(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(Constants.IsSystemAdmin, user.IsSystemAdmin.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return new ClaimsPrincipal(claimsIdentity);
        }

        private string GetOtpSetupCode(string username)
        {
            var secretKey = Utilities.GenerateSecretKey();
            userService.UpdateSecretKey(username, secretKey);

            return twoFactorAuthService.SetupGoogleAuthInfo(sessionService.Username, secretKey);
        }

        private async Task PopulateMenusAndFunctions(User user)
        {
            sessionService.MenuItems = await baseService.GetDataAsync<MenuItem>(
                new GetMenuByUserIdCriteria
                {
                    UserId = user.UserId
                }).ConfigureAwait(false);

            sessionService.Functions = await baseService.GetDataAsync<FunctionModel>(
                new GetFunctionByUserIdCriteria
                {
                    UserId = user.UserId
                }).ConfigureAwait(false);
        }

        private async Task SignUserIn(User user)
        {
            // Step 1: setup userId and username in Session
            sessionService.UserId = user.UserId;
            sessionService.Username = user.Username;

            // Step 2: Get menus and functions
            await PopulateMenusAndFunctions(user);

            // Step 3: sign user in
            ClaimsPrincipal principal = InitUserClaimsPrinciple(user);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            await DoCheckin(user);
        }

        private async Task DoCheckin(User user)
        {
            var criteria = new CheckInUserCriteria
            {
                UserId = user.UserId,
                NetSessionId = HttpContext.Session.Id,
                IP = HttpContext.Connection.RemoteIpAddress.ToString(),
                Platform = Request.Headers["User-Agent"].ToString()
            };

            await baseService.ExecuteNonQueryAsync(criteria);

            sessionService.OnUserId = criteria.OnUserId;
        }
    }
}