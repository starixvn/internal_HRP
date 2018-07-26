namespace HRP.Models.MailSetting
{
    public class EditMailSettingViewModel
    {
        public MailSettingModel MailSetting { get; set; } = new MailSettingModel();
        public bool IsSuccessful { get; set; }
        public string MessageResult { get; set; }
    }
}