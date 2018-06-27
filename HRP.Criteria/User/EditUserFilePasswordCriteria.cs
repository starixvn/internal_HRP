namespace HRP.Criteria.User
{
    using Fanex.Data.Repository;

    public class EditUserFilePasswordCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string Password { get; set; }

        public override string GetSettingKey() => "User_EditFilePassword";

        public override bool IsValid() => UserId > 0 && !string.IsNullOrEmpty(Password);
    }
}