namespace HRP.Criteria
{
    public class UpdateUserLastLoginDateCriteria : HRPCriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "User_UpdateLastLoginDate";

        public override bool IsValid() => UserId > 0;
    }
}