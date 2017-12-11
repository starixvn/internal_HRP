namespace HRP.Criteria.User
{
    using Fanex.Data.Repository;

    public class GetUserCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "User_GetById";

        public override bool IsValid() => UserId > 0;
    }
}
