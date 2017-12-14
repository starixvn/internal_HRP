namespace HRP.Website.Controllers
{
    using HRP.Core.Filters;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}