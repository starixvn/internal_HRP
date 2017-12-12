namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class CheckUsernameExistedCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override string GetSettingKey() => "User_CheckUsernameExisted";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}