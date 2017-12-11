namespace HRP.Website.Controllers
{
    using HRP.Core.Filters;
    using HRP.Core.Services;
    using HRP.Criteria.User;
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc;
    using BCrypt.Net;

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