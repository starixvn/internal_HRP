namespace HRP.Criteria.MonthlyPayroll
{
    using Fanex.Data.Repository;

    public class GetMonthlySalaryToUpdateCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public long PayrollId { get; set; }

        public override string GetSettingKey() => "MonthlyPayroll_GetSalaryToUpdate";

        public override bool IsValid() => UserId > 0 && PayrollId > 0;
    }
}