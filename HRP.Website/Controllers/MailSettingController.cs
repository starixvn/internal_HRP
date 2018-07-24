namespace HRP.Website.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Filters;
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
    }
}