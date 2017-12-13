namespace HRP.Criteria.User
{
    using Fanex.Data.Repository;

    public class UpdateUserProfileCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public override string GetSettingKey() => "User_UpdateProfile";

        public override bool IsValid() => UserId > 0 && !string.IsNullOrEmpty(Email);
    }
}