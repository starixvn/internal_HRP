namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Mail;
    using HRP.Models.MailSetting;

    public class MailSettingService(
        IBaseService baseService,
        ISessionService sessionService) : IMailSettingService
    {
        public async Task<IEnumerable<MailSettingModel>> GetMailSettingAsync()
        {
            var criteria = new GetMailSettingCriteria();

            return await baseService.GetDataAsync<MailSettingModel>(criteria);
        }

        public async Task<MailSettingModel> GetMailSettingByCompanyIdAsync(int companyId)
        {
            var criteria = new GetMailSettingByCompanyIdCriteria
            {
                CompanyId = companyId
            };

            return await baseService.GetDataEntityAsync<MailSettingModel>(criteria);
        }

        public async Task<MailSettingModel> GetMailSettingForSendMailAsync(int companyId)
        {
            var criteria = new GetMailSettingForSendMailCriteria
            {
                CompanyId = companyId
            };

            return await baseService.GetDataEntityAsync<MailSettingModel>(criteria);
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

            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}