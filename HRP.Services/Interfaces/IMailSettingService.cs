namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Models.MailSetting;

    public interface IMailSettingService
    {
        Task<IEnumerable<MailSettingModel>> GetMailSettingAsync();
    }
}