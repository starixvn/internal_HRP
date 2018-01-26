namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BCrypt.Net;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Extensions;
    using Core.Filters;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using Models.UserManagement;
    using Models.UserPermisson;
    using Services.Interfaces;

    [HRPAuthorize]
    public class UserManagementController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;
        private readonly IFunctionService functionService;

        public UserManagementController(
            ISessionService sessionService,
            IBaseService baseService,
            IFunctionService functionService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.functionService = functionService;
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Read)]
        public IActionResult Index()
        {
            var viewModel = new UserManagementViewModel
            {
                CanCreateNewUser = sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host)
            };

            return View(viewModel);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Read)]
        public IActionResult GetUsers(bool? status)
        {
            return ViewComponent("UserTable", new { status });
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Read)]
        public async Task<IActionResult> AssignPermission(int id)
        {
            var functions = await functionService.GetTreeFunctions(isActive: true);
            var companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { IsActive = true });
            var permissions = await baseService.GetDataAsync<UserPermissionModel>(new GetFunctionByUserIdCriteria { UserId = id });
            var user = await baseService.GetDataEntityAsync<UserModel>(new GetUserByIdCriteria { UserId = id });
            var isHost = sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host) && user != null && !user.IsSystemAdmin;

            var model = new AssignPermissonViewModel
            {
                UserId = id,
                Functions = functions,
                Companies = companies,
                UserPermissions = permissions,
                Username = user != null ? user.Username : string.Empty,
                IsHost = isHost
            };

            return View(model);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public async Task<JsonResult> AssignPermission(IEnumerable<UserPermissionModel> permissions)
        {
            var userId = permissions != null && permissions.Any() ? permissions.First().UserId : 0;

            var criteria = new AssignFunctionToUserCriteria
            {
                UserId = userId,
                Value = permissions != null && permissions.Any() ? permissions.ToXML().ToString() : string.Empty,
                ModifiedBy = sessionService.UserId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return Json(commandResult);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public IActionResult CreateUser()
        {
            var viewModel = new CreateUserViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public IActionResult CreateUser(CreateUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var criteria = new CreateUserCriteria
                {
                    Username = viewModel.Username,
                    FullName = viewModel.FullName,
                    Phone = viewModel.PhoneNumber,
                    Email = viewModel.Email,
                    Password = BCrypt.HashPassword(viewModel.Password),
                    IsActive = viewModel.IsActive,
                    CreatedBy = sessionService.UserId
                };

                var result = baseService.ExecuteNonQuery(criteria);
                return Json(new
                {
                    isSuccessful = result.IsSuccessful,
                    result.Message,
                    username = viewModel.Username,
                    redirectTo = result.IsSuccessful && criteria.UserId > 0 ? Url.Action("AssignPermission", new { id = criteria.UserId }) : string.Empty
                });
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };
            return Json(new { isSuccessful = false, errors });
        }

        [HttpGet]
        public IActionResult CheckUsernameExisted(string username)
        {
            return Json(!baseService.GetData<int>(new CheckUsernameExistedCriteria { Username = username }).Any());
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Read)]
        public IActionResult EditUser(int id)
        {
            var viewModel = baseService.GetDataEntity<EditUserViewModel>(new GetUserByIdCriteria { UserId = id });
            if (viewModel == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            viewModel.CanEdit = sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host)
                && (!viewModel.IsSystemAdmin || Convert.ToBoolean(User.FindFirst(x => x.Type == Constants.IsSystemAdmin).Value));

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public IActionResult EditUser(EditUserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var criteria = new EditUserCriteria
                {
                    UserId = viewModel.UserId,
                    FullName = viewModel.FullName,
                    Phone = viewModel.Phone,
                    Email = viewModel.Email,
                    IsActive = viewModel.IsActive,
                    EditedBy = sessionService.UserId
                };

                CommandResult result = baseService.ExecuteNonQuery(criteria);
                return Json(result);
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };
            return Json(new { isSuccessful = false, errors });
        }
    }
}