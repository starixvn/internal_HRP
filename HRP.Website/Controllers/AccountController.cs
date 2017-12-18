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
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.Account;
    using Resources;
    using Services;

    [HRPAuthorize]
    public class AccountController : Controller
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;
        private readonly ITwoFactorAuthenticatorService twoFactorAuthService;
        private readonly IUserService userService;

        public AccountController(
            ISessionService sessionService,
            IBaseService baseService,
            ITwoFactorAuthenticatorService twoFactorAuthService,
            IUserService userService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.twoFactorAuthService = twoFactorAuthService;
            this.userService = userService;
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel viewlModel)
        {
            if (ModelState.IsValid)
            {
                var user = userService.GetUserByUsername(sessionService.Username);
                var isValidOldPassword = BCrypt.Verify(viewlModel.OldPassword, user.Password);

                if (!isValidOldPassword)
                {
                    return Json(new { code = -2, message = Message.OldPasswordIncorrect });
                }

                var criteria = new UpdateUserPasswordCriteria
                {
                    UserId = sessionService.UserId,
                    Password = BCrypt.HashPassword(viewlModel.Password)
                };

                var result = baseService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    return Json(new { code = result.Code, message = Message.PasswordChangeSuccessful });
                }
            }

            return Json(new { code = -1, message = Message.UpdateFail });
        }

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

        [AllowAnonymous]
        public IActionResult RegisterOTPNotification(string name)
        {
            return View("RegisterOTPNotification", name);
        }

        public IActionResult ResetOtpDevice()
        {
            // step 1: Reset IsOTPRegistered
            userService.UpdateOTPStatus(sessionService.UserId, false);

            // step 2: Reset OTP token
            var otpRegisterToken = userService.GenerateTokenOTPRegister(sessionService.Username);

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
            var user = userService.GetUserByTokenOTPRegister(token);

            if (string.IsNullOrEmpty(token) || user == null)
            {
                return BadRequest();
            }

            var setupOtpViewModel = new SetupOtpViewModel
            {
                TextCode = GetOtpSetupCode(user.Username),
                Username = user.Username
            };

            return View(setupOtpViewModel);
        }

        [HttpPost]
        public IActionResult UpdateProfile(UserProfileViewModel viewModel)
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

                var result = baseService.ExecuteNonQuery(criteria);

                if (result.IsSuccessful)
                {
                    return Json(new { code = result.Code, message = Message.UpdateSuccess });
                }
            }

            return Json(new { code = -1, message = Message.UpdateFail });
        }

        [AllowAnonymous]
        public IActionResult VerifyOTP()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }

            if (string.IsNullOrEmpty(sessionService.Username))
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

            var user = userService.GetUserByUsername(sessionService.Username);

            if (!ModelState.IsValid || user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var isOTPValid = twoFactorAuthService.ValidateOTPCode(user.SecretKey, viewModel.OTPCode);
            
            // TODO: Remove IsByPassTwoFactor
            if (isOTPValid || viewModel.IsByPassTwoFactor)
            {
                await SignUserIn(user);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.ErrorMessage = Message.InvalidOTP;

            return RedirectToAction(nameof(VerifyOTP));
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult VerifySetupOTP(SetupOtpViewModel viewModel)
        {
            var user = userService.GetUserByUsername(viewModel.Username);

            if (!ModelState.IsValid || user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var isOTPValid = twoFactorAuthService.ValidateOTPCode(user.SecretKey, viewModel.Otp);

            if (!isOTPValid)
            {
                return BadRequest();
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
                new Claim(ClaimTypes.GivenName, user.FullName),
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

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal)
                .ConfigureAwait(false);

            // Step 4: Update last login date
            baseService.ExecuteNonQuery(new UpdateUserLastLoginDateCriteria { UserId = sessionService.UserId });
        }
    }
}