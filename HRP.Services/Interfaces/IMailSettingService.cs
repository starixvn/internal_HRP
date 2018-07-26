namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.MailSetting;

    public interface IMailSettingService
    {
        Task<IEnumerable<MailSettingModel>> GetMailSettingAsync();

        Task<MailSettingModel> GetMailSettingByCompanyIdAsync(int companyId);

        Task<CommandResult> EditMailSettingAsync(MailSettingModel mailSetting);
    }
}