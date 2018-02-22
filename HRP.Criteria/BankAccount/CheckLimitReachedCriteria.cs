namespace HRP.Criteria.BankAccount
{
    using Fanex.Data.Repository;

    public class CheckLimitReachedCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "BankAccount_CheckLimitReached";

        public override bool IsValid() => EmployeeId > 0;
    }
}