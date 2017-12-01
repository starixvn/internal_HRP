namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetMenuByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override bool IsValid() => UserId > 0;

        public override string GetSettingKey() => "Menu_GetByUserId";
    }
}