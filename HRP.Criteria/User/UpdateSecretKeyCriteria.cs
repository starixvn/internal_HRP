namespace HRP.Criteria
{
    public class UpdateSecretKeyCriteria : HRPCriteriaBase
    {
        public string Username { get; set; }

        public string SecretKey { get; set; }

        public override string GetSettingKey() => "User_UpdateSecretKey";

        public override bool IsValid() => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(SecretKey);
    }
}