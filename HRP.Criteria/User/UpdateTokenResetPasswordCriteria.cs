namespace HRP.Criteria
{
    public class UpdateTokenResetPasswordCriteria : BaseNonQueryCommand
    {
        public string Username { get; set; }

        public string TokenResetPassword { get; set; }

        public override string GetSettingKey() => "User_UpdateTokenResetPassword";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
    }
}