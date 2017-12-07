namespace HRP.Criteria.User
{
    using Fanex.Data.Repository;

    public class UpdateUserPasswordCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string Password { get; set; }

        public override string GetSettingKey() => "User_UpdatePassword";

        public override bool IsValid() => !string.IsNullOrEmpty(Password) && UserId != 0;
    }
}