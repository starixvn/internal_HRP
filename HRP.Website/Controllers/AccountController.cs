namespace HRP.Website.Controllers
{
    using BCrypt.Net;
    using Core.Filters;
    using Core.Services;
    using Criteria.User;
    using HRP.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class AccountController : Controller
    {
        private readonly ISessionService _sessionService;

        private readonly IBaseService _baseService;

        public AccountController(ISessionService sessionService, IBaseService baseService)
        {
            _sessionService = sessionService;
            _baseService = baseService;
        }

        [AllowAnonymous]
        public IActionResult Index(string activePanel)
        {
            return View(activePanel);
        }

        public IActionResult Profile()
        {
            return View("_Profile");
        }

        public IActionResult Password()
        {
            return View("_Password");
        }

        public IActionResult OTP()
        {
            return View("_OTP");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        public IActionResult ChangePassword(ChangePasswordViewModel viewlModel)
        {
            if (ModelState.IsValid)
            {
                var criteria = new UpdateUserPasswordCriteria
                {
                    UserId = _sessionService.UserId,
                    Password = BCrypt.HashPassword(viewlModel.Password)
                };

                var result = _baseService.ExecuteNonQuery(criteria);
            }

            return View();
        }
    }
}