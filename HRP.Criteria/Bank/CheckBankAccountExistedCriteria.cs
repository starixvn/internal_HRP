namespace HRP.Criteria.Bank
{
    using Fanex.Data.Repository;

    public class CheckBankAccountExistedCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "BankAccount_CheckExisted";

        public override bool IsValid()
        {
            if (EmployeeId == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(AccountName) || string.IsNullOrEmpty(AccountNumber))
            {
                return false;
            }

            return true;
        }
    }
}