
namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateTokenResetPasswordCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public string TokenResetPassword { get; set; }

        public override string GetSettingKey() => "User_UpdateTokenResetPassword";

        public override bool IsValid() => !string.IsNullOrEmpty(Username);
        
    }
}
