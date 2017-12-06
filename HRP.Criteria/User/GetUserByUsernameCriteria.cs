namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetUserByUsernameCriteria : CriteriaBase
    {
        public string Username { get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Username);

        public override string GetSettingKey() => "User_GetByUsername";
    }
}