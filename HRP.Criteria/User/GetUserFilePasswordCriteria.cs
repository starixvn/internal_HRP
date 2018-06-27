namespace HRP.Criteria.User
{
    using Fanex.Data.Repository;

    public class GetUserFilePasswordCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override string GetSettingKey() => "User_GetFilePassword";

        public override bool IsValid() => UserId > 0;
    }
}