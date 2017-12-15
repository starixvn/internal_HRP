namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class AssignFunctionToUserCriteria : HRPCriteriaBase
    {
        public int UserId { get; set; }

        public string Value { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid() => UserId > 0 && ModifiedBy > 0 && !string.IsNullOrWhiteSpace(Value);

        public override string GetSettingKey() => "User_AssignFunction";
    }
}