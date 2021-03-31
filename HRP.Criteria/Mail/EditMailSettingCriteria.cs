namespace HRP.Criteria.Mail
{
    public class EditMailSettingCriteria : BaseNonQueryCommand
    {
        public int CompanyId { get; set; }
        public string Server { get; set; }
        public int? Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAuth { get; set; }
        public int UpdatedBy { get; set; }

        public override string GetSettingKey() => "MailSetting_Update";

        public override bool IsValid() => UpdatedBy > 0;
    }
}