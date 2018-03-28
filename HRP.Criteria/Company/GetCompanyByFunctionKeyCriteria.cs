namespace HRP.Criteria
{
    using Fanex.Data.Repository;

    public class GetCompanyByFunctionKeyCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public string FunctionKey { get; set; }

        public override bool IsValid() => UserId > 0 && !string.IsNullOrWhiteSpace(FunctionKey);

        public override string GetSettingKey() => "Company_GetByFunctionKey";
    }
}