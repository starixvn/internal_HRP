namespace HRP.Website.Controllers
{
    using System;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.User;
    using Microsoft.AspNetCore.Mvc;

    public class UserManagementController : Controller
    {
        private readonly IBaseService _baseService;

        public DateTime DateTime { get; private set; }

        public UserManagementController(IBaseService baseService)
        {
            _baseService = baseService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetUsers(bool? status)
        {
            var users = _baseService.GetData<UserModel>(new GetUsersCriteria { IsActive = status });
            return PartialView("_Users", users);
        }
    }
}