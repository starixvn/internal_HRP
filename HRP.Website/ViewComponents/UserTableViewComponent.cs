namespace HRP.Website.ViewComponents
{
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc;

    public class UserTableViewComponent : ViewComponent
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public UserTableViewComponent(ISessionService sessionService, IBaseService baseService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
        }

        public async Task<IViewComponentResult> InvokeAsync(bool? status)
        {
            var criteria = new GetUsersCriteria { IsActive = status };
            var users = await baseService.GetDataAsync<UserModel>(criteria);
            ViewBag.CanEditUserInfo = sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host);

            return View("UserTable", users);
        }
    }
}