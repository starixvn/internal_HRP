namespace HRP.Models.MailSetting
{
    using System.Collections.Generic;

    public class MailSettingViewModel
    {
        public IEnumerable<MailSettingModel> MailSettings { get; set; } = new List<MailSettingModel>();
        public bool IsHost { get; set; }
    }
}