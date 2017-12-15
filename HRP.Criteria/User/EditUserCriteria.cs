namespace HRP.Criteria
{
    public class EditUserCriteria : HRPCriteriaBase
    {
        public int UserId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool IsActive { get; set; }

        public int EditedBy { get; set; }

        public override string GetSettingKey() => "User_EditUser";

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Email) && UserId > 0 && EditedBy > 0;
        }
    }
}