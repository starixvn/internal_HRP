namespace HRP.Criteria.BankAccount
{
    using Fanex.Data.Repository;

    public class GetBankAccountsByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int? BankTypeId { get; set; }

        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "BankAccount_GetBankAccountsByEmpId";

        public override bool IsValid() => EmployeeId > 0;
    }
}