namespace HRP.Website.Controllers
{
    using HRP.Core.Extensions;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System.IO;
    using System.Threading.Tasks;

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

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            var result = new CommandResult();

            if (image != null && image.Length != 0)
            {
                var path = Path.Combine(
                            Directory.GetCurrentDirectory(), "wwwroot",
                            image.GetFilename());

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                    result.Code = 1;
                    result.Message = "Successful!!!";
                }
            }

            return Json(result);
        }
    }
}