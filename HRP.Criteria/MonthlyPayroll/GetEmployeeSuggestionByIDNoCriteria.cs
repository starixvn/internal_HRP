namespace HRP.Criteria.MonthlyPayroll
{
    using Fanex.Data.Repository;

    public class GetEmployeeSuggestionByIDNoCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public int CompanyId { get; set; }

        public string IDNo { get; set; }

        public override string GetSettingKey() => "MonthlyPayroll_GetEmployeeSuggestionByIDNo";

        public override bool IsValid() => UserId > 0 && CompanyId > 0;
    }
}