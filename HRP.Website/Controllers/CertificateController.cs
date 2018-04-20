namespace HRP.Website.Controllers
{
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.RefResource;
    using HRP.Models.Certificate;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class CertificateController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;
        private readonly IZipFileWrapper zipFileWrapper;
        private readonly ICertificateService certificateService;

        public CertificateController(
            ISessionService sessionService,
            IBaseService baseService,
            IZipFileWrapper zipFileWrapper,
            ICertificateService certificateService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.zipFileWrapper = zipFileWrapper;
            this.certificateService = certificateService;
        }

        public IActionResult Generate()
        {
            var viewModel = new GenerateCertificateViewModel
            {
                IsSystemAdmin = sessionService.IsSystemAdmin
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Generate(GenerateCertificateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = certificateService.GenerateSelfSignedCertificate(viewModel.Password);

                if (commandResult.IsSuccessful)
                {
                    viewModel.IsIsSuccessful = commandResult.IsSuccessful;
                    certificateService.CachePasswordAccessTheCertificate(viewModel.Password);

                    await UpdateIsGeneratedCertificate();
                }
                else
                {
                    viewModel.IsIsSuccessful = commandResult.IsSuccessful;
                    viewModel.MessageResult = commandResult.Message;
                }
            }

            return View(viewModel);
        }

        public IActionResult Password()
        {
            var viewModel = new GenerateCertificateViewModel
            {
                IsSystemAdmin = sessionService.IsSystemAdmin
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Password(GenerateCertificateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (certificateService.IsCorrectPasswordAccessTheCertificate(viewModel.Password))
                {
                    certificateService.CachePasswordAccessTheCertificate(viewModel.Password);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(nameof(viewModel.Password), Message.PasswordNotMatch);
                }
            }

            return View(viewModel);
        }

        public IActionResult ChangePassword()
        {
            var viewModel = new ChangeCertificatePasswordViewModel
            {
                IsSystemAdmin = sessionService.IsSystemAdmin
            };

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult ChangePassword(ChangeCertificatePasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (certificateService.IsCorrectPasswordAccessTheCertificate(viewModel.OldPassword))
                {
                    CommandResult commandResult = certificateService.ChangePasswordAccessTheCertificate(viewModel.OldPassword, viewModel.Password);

                    if (commandResult.IsSuccessful)
                    {
                        viewModel.IsIsSuccessful = commandResult.IsSuccessful;
                        certificateService.CachePasswordAccessTheCertificate(viewModel.Password);
                    }
                    else
                    {
                        viewModel.IsIsSuccessful = commandResult.IsSuccessful;
                        viewModel.MessageResult = commandResult.Message;
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(viewModel.Password), Message.PasswordNotMatch);
                }
            }

            return View(viewModel);
        }

        public IActionResult FileNotFound()
        {
            ViewData["IsSystemAdmin"] = sessionService.IsSystemAdmin;
            return View();
        }

        public IActionResult Download()
        {
            string filePath = certificateService.GetCertificateFilePath();

            return File(zipFileWrapper.CopyFileToByteArray(filePath, "HRP.pfx"), MimeType.ZIP, "HRP.zip");
        }

        private async Task UpdateIsGeneratedCertificate()
        {
            var criteria = new UpdateRefResourceItemCriteria
            {
                ResourceId = 1,
                RefName = ReferenceResource.IsGeneratedCertificate,
                Value = "1",
                IsActive = true,
                UpdatedBy = sessionService.UserId
            };

            await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}