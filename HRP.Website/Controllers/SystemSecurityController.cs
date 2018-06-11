namespace HRP.Website.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.RefResource;
    using HRP.Models.SystemSecurity;
    using HRP.Resources;
    using HRP.Services;
    using Microsoft.AspNetCore.Mvc;

    [HRPAuthorize]
    public class SystemSecurityController : Controller
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;
        private readonly ICertificateService certificateService;
        private readonly IRijndaelService rijndaelService;

        public SystemSecurityController(
            ISessionService sessionService,
            IBaseService baseService,
            ICertificateService certificateService,
            IRijndaelService rijndaelService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.certificateService = certificateService;
            this.rijndaelService = rijndaelService;
        }

        [HRPPermission(FunctionKey = FunctionKey.SecuritySetting, Policy = Policy.Host)]
        public IActionResult Index()
        {
            var viewModel = new ChangeCertificatePasswordViewModel
            {
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.SecuritySetting, Policy.Host)
            };

            return View(viewModel);
        }

        public IActionResult Generate()
        {
            Tuple<string, string> tuple = rijndaelService.GenerateKey();
            sessionService.RijndaelKey = tuple.Item1;
            sessionService.RijndaelIV = tuple.Item2;
            var viewModel = new GenerateCertificateViewModel
            {
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.SecuritySetting, Policy.Host),
                RijndaelIV = sessionService.RijndaelIV,
                RijndaelKey = sessionService.RijndaelKey
            };

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.SecuritySetting, Policy = Policy.Host)]
        public async Task<IActionResult> Generate(GenerateCertificateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Password = rijndaelService.Decrypt(viewModel.Password, sessionService.RijndaelKey, sessionService.RijndaelIV);
                CommandResult commandResult = certificateService.GenerateSelfSignedCertificate(viewModel.Password);
                viewModel.IsSuccessful = commandResult.IsSuccessful;

                if (commandResult.IsSuccessful)
                {
                    sessionService.RijndaelIV = string.Empty;
                    sessionService.RijndaelKey = string.Empty;
                    certificateService.CachePasswordAccessTheCertificate(viewModel.Password);

                    await UpdateIsGeneratedCertificate();
                }
                else
                {
                    viewModel.MessageResult = commandResult.Message;
                }
            }

            Tuple<string, string> tuple = rijndaelService.GenerateKey();
            sessionService.RijndaelKey = tuple.Item1;
            sessionService.RijndaelIV = tuple.Item2;
            viewModel.RijndaelIV = sessionService.RijndaelIV;
            viewModel.RijndaelKey = sessionService.RijndaelKey;

            return View(viewModel);
        }

        public IActionResult Password()
        {
            Tuple<string, string> tuple = rijndaelService.GenerateKey();
            sessionService.RijndaelKey = tuple.Item1;
            sessionService.RijndaelIV = tuple.Item2;
            var viewModel = new InputPasswordViewModel
            {
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.SecuritySetting, Policy.Host),
                RijndaelIV = sessionService.RijndaelIV,
                RijndaelKey = sessionService.RijndaelKey
            };

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.SecuritySetting, Policy = Policy.Host)]
        public IActionResult Password(InputPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                viewModel.Password = rijndaelService.Decrypt(viewModel.Password, sessionService.RijndaelKey, sessionService.RijndaelIV);
                if (certificateService.IsCorrectPasswordAccessTheCertificate(viewModel.Password))
                {
                    certificateService.CachePasswordAccessTheCertificate(viewModel.Password);
                    sessionService.RijndaelIV = string.Empty;
                    sessionService.RijndaelKey = string.Empty;

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(nameof(viewModel.Password), Message.PasswordNotMatch);
                }
            }

            Tuple<string, string> tuple = rijndaelService.GenerateKey();
            sessionService.RijndaelKey = tuple.Item1;
            sessionService.RijndaelIV = tuple.Item2;
            viewModel.RijndaelIV = sessionService.RijndaelIV;
            viewModel.RijndaelKey = sessionService.RijndaelKey;

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.SecuritySetting, Policy = Policy.Host)]
        public IActionResult ChangePassword(ChangeCertificatePasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                if (certificateService.IsCorrectPasswordAccessTheCertificate(viewModel.OldPassword))
                {
                    CommandResult commandResult = certificateService.ChangePasswordAccessTheCertificate(viewModel.OldPassword, viewModel.Password);

                    if (commandResult.IsSuccessful)
                    {
                        certificateService.CachePasswordAccessTheCertificate(viewModel.Password);
                    }

                    return Json(commandResult);
                }
                else
                {
                    ModelState.AddModelError(nameof(viewModel.OldPassword), Message.OldPasswordIncorrect);
                }
            }

            var errors = ModelState.Where(x => x.Value.Errors.Any()).ToDictionary(x => x.Key, x => x.Value.Errors.First().ErrorMessage);
            return Json(new { isSuccessful = false, errors });
        }

        public IActionResult FileNotFound()
        {
            ViewData["IsHost"] = sessionService.HasPermissionOnFunction(FunctionKey.SecuritySetting, Policy.Host);
            return View();
        }

        [HRPPermission(FunctionKey = FunctionKey.SecuritySetting, Policy = Policy.Host)]
        public IActionResult Download()
        {
            byte[] certData = certificateService.CopyCertificateFileToByteArray();

            return File(certData, MimeType.PFX, Constants.CertificateFileName);
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