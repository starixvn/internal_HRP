namespace HRP.Website.Controllers
{
    using BCrypt.Net;
    using Core.Filters;
    using Core.Services;
    using Criteria.User;
    using HRP.Models.Account;
    using HRP.Models.UserProfile;
    using HRP.Resources;
    using HRP.Services;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class AccountController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly IBaseService _baseService;
        private readonly IUserService _userService;

        public AccountController(
            ISessionService sessionService,
            IBaseService baseService,
            IUserService userService)
        {
            _sessionService = sessionService;
            _baseService = baseService;
            _userService = userService;
        }

        [AllowAnonymous]
        public IActionResult Index(AccountViewModel viewModel)
        {
            return View(viewModel);
        }

        public IActionResult Profile()
        {
            return PartialView("_Password");
        }

        public IActionResult Password()
        {
            return PartialView("_Password");
        }

        public IActionResult OTP()
        {
            return PartialView("_OTP");
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