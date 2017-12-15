namespace HRP.Criteria.User
{
    public class UpdateUserProfileCriteria : CommandBaseCriteria
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public override string GetSettingKey() => "User_UpdateProfile";

        public override bool IsValid() => UserId > 0 && !string.IsNullOrEmpty(Email);
    }
}