namespace HRP.Criteria.MonthlyPayroll
{
    using Fanex.Data.Repository;

    public class GetEmployeeSuggestionByNameCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public string Name { get; set; }

        public override string GetSettingKey() => "MonthlyPayroll_GetEmployeeSuggestionByName";

        public override bool IsValid() => UserId > 0 && CompanyId > 0;
    }
}