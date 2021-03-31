namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class CheckInUserCriteria : BaseNonQueryCommand
    {
        public int UserId { get; set; }

        public int OnUserId { get; set; }

        public string IP { get; set; }

        public string Platform { get; set; }

        public string NetSessionId { get; set; }

        public override bool IsValid() => UserId > 0 && !string.IsNullOrWhiteSpace(NetSessionId);

        public override string GetSettingKey() => "User_CheckIn";
    }
}