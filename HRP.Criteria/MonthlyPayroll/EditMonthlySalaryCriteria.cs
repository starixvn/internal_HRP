namespace HRP.Criteria.MonthlyPayroll
{
    public class EditMonthlySalaryCriteria : CommandBaseCriteria
    {
        public override string GetSettingKey() => "MonthlyPayroll_UpdateSalary";

        public override bool IsValid() => true;
    }
}