namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Models.MailSetting;
    using HRP.Services;
    using Microsoft.AspNetCore.Mvc;

    public class MailSettingController : BaseController
    {
        private readonly IMailSettingService mailSettingService;

        public MailSettingController(
            ISessionService sessionService,
            IBaseService baseService,
            IMailSettingService mailSettingService) : base(baseService, sessionService)
        {
            this.mailSettingService = mailSettingService;
        }

        [HRPPermission(FunctionKey = FunctionKey.Payslip, Policy = Policy.Read)]
        public async Task<IActionResult> Index()
        {
            var isHost = sessionService.HasPermissionOnFunction(FunctionKey.MailSetting, Policy.Host);
            IEnumerable<MailSettingModel> mailSettings = await mailSettingService.GetMailSettingAsync();
            var viewModel = new MailSettingViewModel
            {
                MailSettings = mailSettings,
                IsHost = isHost
            };

            return View(viewModel);
        }

        [HRPPermission(FunctionKey = FunctionKey.Payslip, Policy = Policy.Host)]
        public async Task<IActionResult> Edit(int id)
        {
            MailSettingModel mailSetting = await mailSettingService.GetMailSettingByCompanyIdAsync(id);

            if (mailSetting == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }

            var viewModel = new EditMailSettingViewModel
            {
                MailSetting = mailSetting
            };

            return View(viewModel);
        }

        [HttpPost]
        [HRPPermission(FunctionKey = FunctionKey.Payslip, Policy = Policy.Read)]
        public async Task<IActionResult> Edit(EditMailSettingViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CommandResult commandResult = await mailSettingService.EditMailSettingAsync(viewModel.MailSetting);
                viewModel.IsSuccessful = commandResult.IsSuccessful;
                viewModel.MessageResult = commandResult.Message;
            }

            return View(viewModel);
        }
    }
}