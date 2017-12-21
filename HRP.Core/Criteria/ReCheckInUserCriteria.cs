namespace HRP.Core.Criteria
{
    using Fanex.Data.Repository;

    public class ReCheckInUserCriteria : CriteriaBase
    {
        public int OnUserId { get; set; }

        public bool IsKickedOut { get; set; }

        public override bool IsValid() => OnUserId > 0;

        public override string GetSettingKey() => "User_ReCheckIn";
    }
}