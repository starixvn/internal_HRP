namespace HRP.Criteria
{
    public class UpdateUserPasswordCriteria : HRPCriteriaBase
    {
        public int UserId { get; set; }

        public string Password { get; set; }

        public override string GetSettingKey() => "User_UpdatePassword";

        public override bool IsValid() => !string.IsNullOrEmpty(Password) && UserId != 0;
    }
}