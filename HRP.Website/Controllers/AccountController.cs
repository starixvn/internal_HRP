namespace HRP.Website.Controllers
{
    using BCrypt.Net;
    using Core.Filters;
    using Core.Services;
    using Criteria.User;
    using HRP.Models;
    using HRP.Models.Account;
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
        public IActionResult Index(AccountViewModel viewModel)
        {
            return View(viewModel);
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