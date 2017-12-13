namespace HRP.Website.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using BCrypt.Net;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.UserManagement;
    using HRP.Models.UserPermisson;
    using HRP.Services;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

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
            return PartialView("_GetUsers", status);
        }

        [HttpPost]
        public IActionResult UpdateUserStatus(int userId, bool isActive)
        {
            var criteria = new UpdateUserStatusCriteria
            {
                UserId = userId,
                IsActive = isActive,
                EditedBy = sessionService.UserId
            };
            var result = baseService.ExecuteNonQuery(criteria);
            if (result.IsSuccessful)
            {
                return Json(new { userId, isActive });
            }

            return StatusCode(500);
        }

        [HttpGet]
        [HRPPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Read)]
        public async Task<IActionResult> AssignPermission(int id)
        {
            var functions = await functionService.GetTreeFunctions(true).ConfigureAwait(false);
            var companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { IsActive = true }).ConfigureAwait(false);
            var permissions = await baseService.GetDataAsync<UserPermissionModel>(new GetFunctionByUserIdCriteria { UserId = id }).ConfigureAwait(false);
            var user = await baseService.GetDataEntityAsync<UserModel>(new GetUserByIdCriteria { UserId = id }).ConfigureAwait(false);
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
        public async Task<JsonResult> AssignPermission(string permissions)
        {
            var userPermissions = !string.IsNullOrWhiteSpace(permissions)
                ? JsonConvert.DeserializeObject<IEnumerable<UserPermissionModel>>(permissions) : new List<UserPermissionModel>();
            var userId = userPermissions.Any() ? userPermissions.First().UserId : 0;

            var criteria = new AssignFunctionToUserCriteria
            {
                UserId = userId,
                Value = userPermissions.ToXML().ToString(),
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
        [ValidateAntiForgeryToken]
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
                    CreatedDate = DateTime.Now,
                    CreatedBy = sessionService.UserId
                };

                var result = baseService.ExecuteNonQuery(criteria);
                if (result.IsSuccessful)
                {
                    return Json(new { isSuccessful = result.IsSuccessful, result.Message, username = viewModel.Username, redirectTo = Url.Action("AssignPermission", new { id = criteria.UserId }) });
                }
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
        [HRPPermission(FunctionKey = FunctionKey.UserManagement, Policy = Policy.Host)]
        public IActionResult EditUser(int id)
        {
            var viewModel = baseService.GetDataEntity<EditUserViewModel>(new GetUserByIdCriteria { UserId = id });
            if (viewModel.IsSystemAdmin && !Convert.ToBoolean(User.FindFirst(x => x.Type == Constants.IsSystemAdmin).Value))
            {
                return RedirectToAction("Index");
            }

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

                var result = baseService.ExecuteNonQuery(criteria);
                return Json(result);
            }

            var errors = from item in ModelState
                         where item.Value.Errors.Any()
                         select new { item.Key, item.Value.Errors.FirstOrDefault().ErrorMessage };
            return Json(new { isSuccessful = false, errors });
        }
    }
}