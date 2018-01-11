namespace HRP.Criteria.Bank
{
    using Fanex.Data.Repository;

    public class GetBankAccountsByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "BankAccount_GetBankAccountsByEmpId";

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}
