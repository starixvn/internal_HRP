namespace HRP.Criteria.MonthlyPayroll
{
    using Fanex.Data.Repository;

    public class GetMonthlyAllowanceCriteria :CriteriaBase
    {
        public long PayrollId { get; set; }

        public override string GetSettingKey() => "MonthlyPayroll_GetAllowances";

        public override bool IsValid() => PayrollId > 0;
    }
}
