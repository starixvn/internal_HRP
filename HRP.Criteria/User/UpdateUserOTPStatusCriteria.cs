namespace HRP.Criteria
{
    public class UpdateUserOTPStatusCriteria : HRPCriteriaBase
    {
        public int UserId { get; set; }

        public bool IsOTPRegistered { get; set; }

        public override string GetSettingKey() => "User_UpdateUserOTPStatus";

        public override bool IsValid() => UserId > 0;
    }
}