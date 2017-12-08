namespace HRP.Criteria.User
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateTokenOTPRegisterCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public string TokenRegisterOTP { get; set; }

        public override string GetSettingKey() => "User_UpdateTokenOTPRegister";

        public override bool IsValid() => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(TokenRegisterOTP);
    }
}