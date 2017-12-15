namespace HRP.Criteria
{
    public class UpdateTokenOTPRegisterCriteria : CommandBaseCriteria
    {
        public string Username { get; set; }

        public string TokenOTPRegister { get; set; }

        public override string GetSettingKey() => "User_UpdateTokenOTPRegister";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}