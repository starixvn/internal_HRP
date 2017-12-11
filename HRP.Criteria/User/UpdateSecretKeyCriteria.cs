namespace HRP.Criteria.User
{
    using Fanex.Data.Repository;

    public class UpdateSecretKeyCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public string SecretKey { get; set; }

        public override string GetSettingKey() => "User_UpdateSecretKey";

        public override bool IsValid() => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(SecretKey);
    }
}