namespace HRP.Criteria.User
{
    using Fanex.Data.Repository;

    public class UpdateUserStatusCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public bool IsActive { get; set; }

        public override string GetSettingKey() => "User_UpdateUserStatus";

        public override bool IsValid() => UserId > 0;
    }
}