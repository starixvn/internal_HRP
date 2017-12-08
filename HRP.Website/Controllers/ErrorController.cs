namespace HRP.Website.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class ErrorController : Controller
    {
        [Route("404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [Route("405")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("500")]
        public IActionResult InternalError()
        {
            return View();
        }
    }
}