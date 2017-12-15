namespace HRP.Criteria
{
    public class UpdateUserOTPStatusCriteria : CommandBaseCriteria
    {
        public int UserId { get; set; }

        public bool IsOTPRegistered { get; set; }

        public override string GetSettingKey() => "User_UpdateUserOTPStatus";

        public override bool IsValid() => UserId > 0;
    }
}