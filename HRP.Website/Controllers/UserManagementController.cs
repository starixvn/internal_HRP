namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using HRP.Core.Services;
    using HRP.Criteria.User;
    using HRP.Models;
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

        [HttpPost]
        public IActionResult UpdateUserStatus(int userId, bool isActive)
        {
            var result = _baseService.ExecuteNonQuery(new UpdateUserStatusCriteria { UserId = userId, IsActive = isActive });
            if (result.IsSuccessful)
            {
                return Json(new { userId, isActive });
            }

            return StatusCode(500);
        }
    }
}