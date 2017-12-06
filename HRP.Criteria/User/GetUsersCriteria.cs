namespace HRP.Criteria.User
{
    using Fanex.Data.Repository;

    public class GetUsersCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "User_GetUsers";

        public override bool IsValid() => true;
    }
}