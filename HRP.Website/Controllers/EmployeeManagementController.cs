namespace HRP.Website.Controllers
{
    using HRP.Core.Filters;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class EmployeeManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateEmployee()
        {
            return View("PersonalDetail");
        }
    }
}