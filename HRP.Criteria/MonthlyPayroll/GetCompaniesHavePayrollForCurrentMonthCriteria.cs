namespace HRP.Criteria.MonthlyPayroll
{
    using Fanex.Data.Repository;

    public class GetCompaniesHavePayrollForCurrentMonthCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "MonthlyPayroll_GetCompaniesHavePayrollForCurrentMonth";

        public override bool IsValid() => true;
    }
}
