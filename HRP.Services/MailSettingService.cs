namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using HRP.Criteria.Mail;
    using HRP.Models.MailSetting;

    public class MailSettingService : IMailSettingService
    {
        private readonly IBaseService baseService;

        public MailSettingService(
            IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<MailSettingModel>> GetMailSettingAsync()
        {
            var criteria = new GetMailSettingCriteria();

            return await baseService.GetDataAsync<MailSettingModel>(criteria).ConfigureAwait(false);
        }
    }
}