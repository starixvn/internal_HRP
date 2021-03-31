namespace HRP.Core.Criteria
{
    using Fanex.Data.Repository;

    public class LogOutUserCriteria : NonQueryCommand
    {
        public int OnUserId { get; set; }

        public string IP { get; set; }

        public string Platform { get; set; }

        public override bool IsValid() => OnUserId > 0;

        public override string GetSettingKey() => "User_Logout";
    }
}