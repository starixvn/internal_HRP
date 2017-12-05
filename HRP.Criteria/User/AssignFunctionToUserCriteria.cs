namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class AssignFunctionToUserCriteria : CriteriaBase
    {
        private const string XmlTemplate = "<FunctionPolicy FunctionId='{0}' CompanyId='{1}' IsRead='{2}' IsHost='{3}'></FunctionPolicy>";

        public int UserId { get; set; }

        public string Value { get; set; }

        public override bool IsValid() => UserId > 0 && !string.IsNullOrWhiteSpace(Value);

        public override string GetSettingKey() => "User_AssignFunction";
    }
}