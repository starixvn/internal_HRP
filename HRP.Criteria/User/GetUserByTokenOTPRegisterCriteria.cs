namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetUserByTokenOTPRegisterCriteria : CriteriaBase
    {
        public string TokenOTPRegister { get; set; }

        public override string GetSettingKey() => "User_GetByTokenOTPRegister";

        public override bool IsValid() => !string.IsNullOrEmpty(TokenOTPRegister);
    }
}