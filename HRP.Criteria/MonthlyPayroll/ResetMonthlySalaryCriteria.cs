namespace HRP.Criteria.MonthlyPayroll
{
    public class ResetMonthlySalaryCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "MonthlyPayroll_ResetSalary";

        public override bool IsValid() => true;
    }
}