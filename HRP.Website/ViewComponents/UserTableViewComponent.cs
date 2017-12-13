namespace HRP.Website.ViewComponents
{
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc;

    public class UserTableViewComponent : ViewComponent
    {
        private IBaseService baseService;
        private ISessionService sessionService;

        public UserTableViewComponent(ISessionService sessionService, IBaseService baseService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
        }

        public IViewComponentResult Invoke(bool? status)
        {
            var criteria = new GetUsersCriteria { IsActive = status };
            var users = baseService.GetData<UserModel>(criteria);
            ViewBag.CanEditUserInfo = sessionService.HasPermissionOnFunction(FunctionKey.UserManagement, Policy.Host);

            return View("UserTable", users);
        }
    }
}