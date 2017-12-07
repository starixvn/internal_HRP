namespace HRP.Criteria.User
{
    using Fanex.Data.Repository;

    public class GetUserByTokenResetPasswordCriteria : CriteriaBase
    {
        public string TokenResetPassword { get; set; }

        public override string GetSettingKey() => "User_GetByTokenResetPassword";

        public override bool IsValid() => !string.IsNullOrEmpty(TokenResetPassword);
    }
}