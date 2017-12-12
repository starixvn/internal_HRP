namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateUserLastLoginDateCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "User_UpdateLastLoginDate";

        public override bool IsValid() => UserId > 0;
    }
}