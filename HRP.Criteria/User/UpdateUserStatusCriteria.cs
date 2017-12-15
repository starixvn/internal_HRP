namespace HRP.Criteria
{
    public class UpdateUserStatusCriteria : HRPCriteriaBase
    {
        public int UserId { get; set; }

        public bool IsActive { get; set; }

        public int EditedBy { get; set; }

        public override string GetSettingKey() => "User_UpdateUserStatus";

        public override bool IsValid() => UserId > 0;
    }
}