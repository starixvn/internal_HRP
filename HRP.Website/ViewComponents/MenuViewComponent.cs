namespace HRP.Website.ViewComponents
{
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using Microsoft.AspNetCore.Mvc;

    public class MenuViewComponent : ViewComponent
    {
        private readonly ISessionService sessionService;

        public MenuViewComponent(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            ViewData["Username"] = sessionService.Username;
            var menuItems = await Task.FromResult(sessionService.MenuItems);

            return View("Menu", menuItems);
        }
    }
}