namespace HRP.Criteria.MonthlyPayroll
{
    public class UpdateMonthlySalaryCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "MonthlyPayroll_UpdateSalary";

        public override bool IsValid() => true;
    }
}