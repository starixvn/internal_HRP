using Microsoft.AspNetCore.Mvc;

namespace HRP.Website.Controllers
{
    public class EmployeeManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}