namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.User;
    using HRP.Models;
    using HRP.Models.UserPermisson;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [HRPAuthorize]
    public class UserManagementController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;

        public UserManagementController(
            ISessionService sessionService,
            IBaseService baseService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetUsers(bool? status)
        {
            var criteria = new GetUsersCriteria { IsActive = status };
            var users = baseService.GetData<UserModel>(criteria);

            return PartialView("_Users", users);
        }

        [HttpPost]
        public IActionResult UpdateUserStatus(int userId, bool isActive)
        {
            var criteria = new UpdateUserStatusCriteria
            {
                UserId = userId,
                IsActive = isActive
            };
            var result = baseService.ExecuteNonQuery(criteria);
            if (result.IsSuccessful)
            {
                return Json(new { userId, isActive });
            }

            return StatusCode(500);
        }

        [HttpGet]
        public async Task<IActionResult> AssignPermission(int id)
        {
            var functions = await baseService.GetDataAsync<HRP.Models.UserPermisson.FunctionModel>(new GetAllFunctionCriteria { IsActive = true }).ConfigureAwait(false);
            var companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria { IsActive = true }).ConfigureAwait(false);
            var permissions = await baseService.GetDataAsync<UserPermissionModel>(new GetFunctionByUserIdCriteria { UserId = sessionService.UserId }).ConfigureAwait(false);

            var model = new AssignPermissonViewModel
            {
                UserId = id,
                Functions = functions,
                Companies = companies,
                UserPermissions = permissions
            };

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> AssignPermission(string permissions)
        {
            var userPermissions = !string.IsNullOrWhiteSpace(permissions)
                ? JsonConvert.DeserializeObject<IEnumerable<UserPermissionModel>>(permissions) : new List<UserPermissionModel>();
            var userId = userPermissions.Any() ? userPermissions.First().UserId : 0;

            var criteria = new AssignFunctionToUserCriteria
            {
                UserId = userId,
                Value = ConvertUserPermissionToXml(userPermissions),
                ModifiedBy = sessionService.UserId
            };

            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return Json(commandResult);
        }

        private static string ConvertUserPermissionToXml(IEnumerable<UserPermissionModel> permissions)
        {
            var template = "<FunctionPolicy FunctionId='{0}' CompanyId='{1}' IsRead='{2}' IsHost='{3}'></FunctionPolicy>";
            var result = new StringBuilder();

            if (permissions.Any())
            {
                result.Append(Constants.BeginRootTag);

                foreach (UserPermissionModel permission in permissions)
                {
                    result.AppendFormat(template, permission.FunctionId, permission.CompanyId, permission.IsRead, permission.IsHost);
                }

                result.Append(Constants.EndRootTag);
            }

            return result.ToString();
        }
    }
}