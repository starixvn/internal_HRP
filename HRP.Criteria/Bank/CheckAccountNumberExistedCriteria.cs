namespace HRP.Criteria.Bank
{
    using Fanex.Data.Repository;

    public class CheckAccountNumberExistedCriteria : CriteriaBase
    {
        public int Id { get; set; }

        public string AccountNumber { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "BankAccount_CheckNumberExisted";

        public override bool IsValid()
        {
            if (EmployeeId == 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(AccountNumber) || string.IsNullOrWhiteSpace(AccountNumber))
            {
                return false;
            }

            return true;
        }
    }
}