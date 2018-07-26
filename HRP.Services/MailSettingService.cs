namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Mail;
    using HRP.Models.MailSetting;

    public class MailSettingService : IMailSettingService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public MailSettingService(
            IBaseService baseService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        public async Task<IEnumerable<MailSettingModel>> GetMailSettingAsync()
        {
            var criteria = new GetMailSettingCriteria();

            return await baseService.GetDataAsync<MailSettingModel>(criteria).ConfigureAwait(false);
        }

        public async Task<MailSettingModel> GetMailSettingByCompanyIdAsync(int companyId)
        {
            var criteria = new GetMailSettingByCompanyIdCriteria
            {
                CompanyId = companyId
            };

            return await baseService.GetDataEntityAsync<MailSettingModel>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditMailSettingAsync(MailSettingModel mailSetting)
        {
            var criteria = new EditMailSettingCriteria
            {
                CompanyId = mailSetting.CompanyId,
                Server = mailSetting.Server,
                Port = mailSetting.Port,
                Username = mailSetting.Username,
                Password = mailSetting.Password,
                IsAuth = mailSetting.IsAuth,
                UpdatedBy = sessionService.UserId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}