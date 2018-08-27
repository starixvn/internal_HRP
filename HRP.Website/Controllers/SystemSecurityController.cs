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
    using HRP.Models;
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
            Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair> tuple = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[tuple.Item1.Key] = tuple.Item1.Value;
            TempData[tuple.Item2.Key] = tuple.Item2.Value;
            var viewModel = new GenerateCertificateViewModel
            {
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.SecuritySetting, Policy.Host),
                RijndaelKey = tuple.Item1,
                RijndaelIV = tuple.Item2
            };

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.SecuritySetting, Policy = Policy.Host)]
        public async Task<IActionResult> Generate(GenerateCertificateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var rijndaelKey = (string)TempData[viewModel.RijndaelKey.Key];
                var rijndaelIV = (string)TempData[viewModel.RijndaelIV.Key];
                viewModel.Password = rijndaelService.Decrypt(viewModel.Password, rijndaelKey, rijndaelIV);
                CommandResult commandResult = certificateService.GenerateSelfSignedCertificate(viewModel.Password);
                viewModel.IsSuccessful = commandResult.IsSuccessful;

                if (commandResult.IsSuccessful)
                {
                    certificateService.CachePasswordAccessTheCertificate(viewModel.Password);

                    await UpdateIsGeneratedCertificate();
                }
                else
                {
                    viewModel.MessageResult = commandResult.Message;
                }
            }

            Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair> tuple = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[tuple.Item1.Key] = tuple.Item1.Value;
            TempData[tuple.Item2.Key] = tuple.Item2.Value;

            viewModel.RijndaelKey = tuple.Item1;
            viewModel.RijndaelIV = tuple.Item2;

            return View(viewModel);
        }

        public IActionResult Password()
        {
            Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair> tuple = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[tuple.Item1.Key] = tuple.Item1.Value;
            TempData[tuple.Item2.Key] = tuple.Item2.Value;
            var viewModel = new InputPasswordViewModel
            {
                IsHost = sessionService.HasPermissionOnFunction(FunctionKey.SecuritySetting, Policy.Host),
                RijndaelKey = tuple.Item1,
                RijndaelIV = tuple.Item2
            };

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.SecuritySetting, Policy = Policy.Host)]
        public IActionResult Password(InputPasswordViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var rijndaelKey = (string)TempData[viewModel.RijndaelKey.Key];
                var rijndaelIV = (string)TempData[viewModel.RijndaelIV.Key];
                viewModel.Password = rijndaelService.Decrypt(viewModel.Password, rijndaelKey, rijndaelIV);
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

            Tuple<RijndaelKeyValuePair, RijndaelKeyValuePair> tuple = rijndaelService.GenerateRijndaelKeyValuePair();
            TempData[tuple.Item1.Key] = tuple.Item1.Value;
            TempData[tuple.Item2.Key] = tuple.Item2.Value;

            viewModel.RijndaelKey = tuple.Item1;
            viewModel.RijndaelIV = tuple.Item2;

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