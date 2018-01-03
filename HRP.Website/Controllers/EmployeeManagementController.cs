namespace HRP.Website.Controllers
{
    using System;
    using HRP.Core.Extensions;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Models.EmployeeManagement;
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
            var model = new EmployeePersonalDetail
            {
                GuidToken = Guid.NewGuid()
            };

            return View("PersonalDetail", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEmployee(EmployeePersonalDetail employee)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }

            return View("PersonalDetail", employee);
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile image, string token)
        {
            var result = new CommandResult
            {
                Message = "Invalid picture"
            };

            if (image != null && image.Length != 0 && !string.IsNullOrEmpty(token))
            {
                var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ProfilePicture", "Temp");
                Directory.CreateDirectory(directoryPath);

                var fileName = image.GetFilename();
                var path = Path.Combine(directoryPath, fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                    result.Code = 0;
                    result.Message = "/ProfilePicture/Temp/" + fileName;
                }
            }

            return Json(result);
        }
    }
}