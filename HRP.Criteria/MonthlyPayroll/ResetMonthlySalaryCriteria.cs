namespace HRP.Criteria.MonthlyPayroll
{
    public class ResetMonthlySalaryCriteria : CommandBaseCriteria
    {
        public int PayrollId { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }

        public override string GetSettingKey() => "MonthlyPayroll_ResetSalary";

        public override bool IsValid() => PayrollId > 0 && CompanyId > 0 && UserId > 0;
    }
}