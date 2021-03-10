namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using BCrypt.Net;
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
    using Microsoft.Extensions.Options;
    using Models;
    using Models.Account;
    using Resources;
    using Services;

    public class AccountController : Controller
    {
        private const int OldPasswordIncorrectCode = -2;
        private const string Logout = "Logout";
        private const string Auth = "Auth";
        private const string IndexText = "Index";
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ITwoFactorAuthenticatorService twoFactorAuthService;
        private readonly IUserService userService;
        private readonly IRijndaelService rijndaelService;
        private readonly IOptionsSnapshot<OtpConfig> otpConfig;

        public AccountController(
            ISessionService sessionService,
            IBaseService baseService,
            ITwoFactorAuthenticatorService twoFactorAuthService,
            IUserService userService,
            IRijndaelService rijndaelService,
            IOptionsSnapshot<OtpConfig> otpConfig)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.twoFactorAuthService = twoFactorAuthService;
            this.userService = userService;
            this.rijndaelService = rijndaelService;
            this.otpConfig = otpConfig;
        }

        [HRPAuthorize]
        [ServiceFilter(typeof(HRPActionFilterAttribute))]
        public async Task<IActionResult> Index(AccountTab tab = AccountTab.Profile)
        {
            (var rijndaelKeyPair, var rijndaelIVPair) = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[rijndaelKeyPair.Key] = rijndaelKeyPair.Value;
            TempData[rijndaelIVPair.Key] = rijndaelIVPair.Value;

            var user = await userService.GetUserByUsernameAsync(sessionService.Username);

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
                UserProfileViewModel = userProfile,
                SetupOtpViewModel = new SetupOtpViewModel
                {
                    RijndaelKey = rijndaelKeyPair,
                    RijndaelIV = rijndaelIVPair
                },
                ChangePasswordViewModel = new ChangePasswordViewModel
                {
                    RijndaelKey = rijndaelKeyPair,
                    RijndaelIV = rijndaelIVPair
                },
                RijndaelKey = rijndaelKeyPair,
                RijndaelIV = rijndaelIVPair
            };

            ViewData["Username"] = sessionService.Username;

            return View(accountViewModel);
        }

        [HRPAuthorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel viewModel)
        {
            var rijndaelKey = (string)TempData.Peek(viewModel.RijndaelKey.Key);
            var rijndaelIV = (string)TempData.Peek(viewModel.RijndaelIV.Key);
            viewModel.OldPassword = rijndaelService.Decrypt(viewModel.OldPassword, rijndaelKey, rijndaelIV);
            viewModel.Password = rijndaelService.Decrypt(viewModel.Password, rijndaelKey, rijndaelIV);
            viewModel.PasswordConfirm = rijndaelService.Decrypt(viewModel.PasswordConfirm, rijndaelKey, rijndaelIV);

            if (ModelState.IsValid)
            {
                bool result = await IsCorrectOldPassword(viewModel.OldPassword);
                if (!result)
                {
                    return Json(new { code = OldPasswordIncorrectCode, message = Message.OldPasswordIncorrect });
                }

                result = await IsSuccessUpdatePassword(viewModel.Password);
                if (result)
                {
                    return Json(new { code = 0, message = Message.PasswordChangeSuccessful });
                }
            }

            return Json(new { code = -1, message = Message.UpdateFail });
        }

        private async Task<bool> IsCorrectOldPassword(string oldPassword)
        {
            var user = await userService.GetUserByUsernameAsync(sessionService.Username);
            var isCorrectOldPassword = userService.IsCorrectPassword(oldPassword, user.Password);
            return isCorrectOldPassword;
        }

        private async Task<bool> IsSuccessUpdatePassword(string newPassword)
        {
            var criteria = new UpdateUserPasswordCriteria
            {
                UserId = sessionService.UserId,
                Password = BCrypt.HashPassword(newPassword)
            };

            var result = await baseService.ExecuteNonQueryAsync(criteria);

            return result.IsSuccessful;
        }

        [AllowAnonymous]
        public IActionResult RegisterOTPNotification(string name)
        {
            return View("RegisterOTPNotification", name);
        }

        [HRPAuthorize]
        public async Task<IActionResult> ResetOtpDevice()
        {
            // step 1: Reset IsOTPRegistered
            await userService.UpdateOTPStatusAsync(sessionService.UserId, false);

            // step 2: Reset OTP token
            string otpRegisterToken = await userService.GenerateTokenOTPRegisterAsync(sessionService.Username);

            // step 3: Show SetupOtp screen
            return RedirectToAction("SetupOtpAfterReset", "Account", new { token = otpRegisterToken });
        }

        [AllowAnonymous]
        public async Task<IActionResult> SetupOtp(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction(Logout, Auth);
            }

            User user = await userService.GetUserByTokenOTPRegisterAsync(token);
            if (user == null)
            {
                return RedirectToAction(Logout, Auth);
            }

            (var rijndaelKeyPair, var rijndaelIVPair) = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[rijndaelKeyPair.Key] = rijndaelKeyPair.Value;
            TempData[rijndaelIVPair.Key] = rijndaelIVPair.Value;

            var setupOtpViewModel = new SetupOtpViewModel
            {
                TextCode = await GetOtpSetupCode(user.Username),
                AppName = otpConfig.Value.AppName,
                Username = user.Username,
                RijndaelKey = rijndaelKeyPair,
                RijndaelIV = rijndaelIVPair
            };

            return View("SetupOtp", setupOtpViewModel);
        }

        [HRPAuthorize]
        public async Task<IActionResult> SetupOtpAfterReset(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction(Logout, Auth);
            }

            User user = await userService.GetUserByTokenOTPRegisterAsync(token);
            if (user == null)
            {
                return RedirectToAction(Logout, Auth);
            }

            var userProfile = new UserProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone
            };

            (var rijndaelKeyPair, var rijndaelIVPair) = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[rijndaelKeyPair.Key] = rijndaelKeyPair.Value;
            TempData[rijndaelIVPair.Key] = rijndaelIVPair.Value;

            var setupOtpViewModel = new SetupOtpViewModel
            {
                TextCode = await GetOtpSetupCode(user.Username),
                AppName = otpConfig.Value.AppName,
                Username = user.Username,
                RijndaelKey = rijndaelKeyPair,
                RijndaelIV = rijndaelIVPair
            };

            var accountViewModel = new AccountViewModel
            {
                ActivePanel = AccountTab.Otp,
                IsOTPRegistered = false,
                SetupOtpViewModel = setupOtpViewModel,
                ChangePasswordViewModel = new ChangePasswordViewModel(),
                UserProfileViewModel = userProfile,
                RijndaelKey = rijndaelKeyPair,
                RijndaelIV = rijndaelIVPair
            };

            return View(IndexText, accountViewModel);
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
                return RedirectToAction(IndexText, new { tab = AccountTab.Otp });
            }

            if (string.IsNullOrEmpty(sessionService.Username))
            {
                return RedirectToAction(Logout, Auth);
            }

            (var rijndaelKeyPair, var rijndaelIVPair) = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[rijndaelKeyPair.Key] = rijndaelKeyPair.Value;
            TempData[rijndaelIVPair.Key] = rijndaelIVPair.Value;

            var viewModel = new TwoFactorAuthenticatorViewModel
            {
                RijndaelKey = rijndaelKeyPair,
                RijndaelIV = rijndaelIVPair
            };

            return View("VerifyOTP", viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyOTP(TwoFactorAuthenticatorViewModel viewModel)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction(IndexText, new { tab = AccountTab.Otp });
            }

            User user = await userService.GetUserByUsernameAsync(sessionService.Username);

            if (user == null)
            {
                return RedirectToAction(Logout, Auth);
            }

            var rijndaelKey = (string)TempData[viewModel.RijndaelKey.Key];
            var rijndaelIV = (string)TempData[viewModel.RijndaelIV.Key];
            viewModel.OTPCode = rijndaelService.Decrypt(viewModel.OTPCode, rijndaelKey, rijndaelIV);

            var isOTPValid = twoFactorAuthService.ValidateOTPCode(user.SecretKey, viewModel.OTPCode);

            if (ModelState.IsValid && isOTPValid)
            {
                await SignUserIn(user);
                return RedirectToAction(IndexText, "Home");
            }

            ViewData["ErrorMessage"] = Message.InvalidOTP;

            (var rijndaelKeyPair, var rijndaelIVPair) = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[rijndaelKeyPair.Key] = rijndaelKeyPair.Value;
            TempData[rijndaelIVPair.Key] = rijndaelIVPair.Value;

            viewModel.RijndaelKey = rijndaelKeyPair;
            viewModel.RijndaelIV = rijndaelIVPair;

            return View("VerifyOTP", viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifySetupOTP(SetupOtpViewModel viewModel)
        {
            ModelState.Clear();

            User user = await userService.GetUserByUsernameAsync(viewModel.Username);

            if (user == null)
            {
                return RedirectToAction(Logout, Auth);
            }

            var rijndaelKey = (string)TempData.Peek(viewModel.RijndaelKey.Key);
            var rijndaelIV = (string)TempData.Peek(viewModel.RijndaelIV.Key);
            viewModel.Otp = rijndaelService.Decrypt(viewModel.Otp, rijndaelKey, rijndaelIV);

            var isOTPValid = twoFactorAuthService.ValidateOTPCode(user.SecretKey, viewModel.Otp);

            if (!TryValidateModel(viewModel) || !isOTPValid)
            {
                return Forbid();
            }

            await userService.UpdateOTPStatusAsync(user.UserId, true);
            await userService.ResetTokenOTPRegisterAsync(user.Username);
            sessionService.Username = user.Username;
            return new OkResult();
        }

        private static ClaimsPrincipal InitUserClaimsPrinciple(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            return new ClaimsPrincipal(claimsIdentity);
        }

        private async Task<string> GetOtpSetupCode(string username)
        {
            var secretKey = Guid.NewGuid().ToString("N").Substring(0, 12);
            await userService.UpdateSecretKeyAsync(username, secretKey);

            return twoFactorAuthService.SetupGoogleAuthInfo(sessionService.Username, secretKey);
        }

        private async Task PopulateMenusAndFunctions(User user)
        {
            sessionService.MenuItems = await baseService.GetDataAsync<MenuItem>(
                new GetMenuByUserIdCriteria
                {
                    UserId = user.UserId
                });

            sessionService.Functions = await baseService.GetDataAsync<FunctionModel>(
                new GetFunctionByUserIdCriteria
                {
                    UserId = user.UserId
                });
        }

        private async Task SignUserIn(User user)
        {
            // Step 1: save some infos into session
            sessionService.UserId = user.UserId;
            sessionService.Username = user.Username;
            sessionService.IsSystemAdmin = user.IsSystemAdmin;

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