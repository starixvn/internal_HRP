namespace HRP.Models.MailSetting
{
    public class MailSettingModel : Company
    {
        public string Server { get; set; }
        public int? Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAuth { get; set; }
        public int? Priority { get; set; }
    }
}