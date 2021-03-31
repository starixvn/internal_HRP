namespace HRP.Criteria.User
{
    public class EditUserFilePasswordCriteria : BaseNonQueryCommand
    {
        public int UserId { get; set; }

        public string Password { get; set; }

        public override string GetSettingKey() => "User_UpdateFilePassword";

        public override bool IsValid() => UserId > 0 && !string.IsNullOrEmpty(Password);
    }
}