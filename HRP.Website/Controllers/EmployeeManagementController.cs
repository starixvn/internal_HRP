namespace HRP.Website.Controllers
{
    using Microsoft.AspNetCore.Mvc;

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