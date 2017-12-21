namespace HRP.Website.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class ErrorController : Controller
    {
        [Route("error/404")]
        public IActionResult PageNotFound()
        {
            return View();
        }

        [Route("error/405")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [Route("error/500")]
        public IActionResult InternalError()
        {
            return View();
        }
    }
}