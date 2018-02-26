namespace HRP.Criteria.BankAccount
{
    using Fanex.Data.Repository;

    public class SearchBankAccountCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int BankTypeId { get; set; }

        public bool? IsActive { get; set; }

        public override string GetSettingKey() => "BankAccount_Search";

        public override bool IsValid() => EmployeeId > 0 && BankTypeId > 0;
    }
}
