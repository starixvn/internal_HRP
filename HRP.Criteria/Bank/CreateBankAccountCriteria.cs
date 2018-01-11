namespace HRP.Criteria.Bank
{
    using Fanex.Data.Repository;
    using Models.Bank;

    public class CreateBankAccountCriteria : CriteriaBase, IBankAccount
    {
        public int EmployeeId { get; set; }

        public int BankType { get; set; }

        public string BankName { get; set; }

        public string BankBranch { get; set; }

        public string BankAddress { get; set; }

        public string BankSwiftCode { get; set; }

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public int AccountType { get; set; }

        public string BeneficiaryName { get; set; }

        public bool IsActive { get; set; }

        public override string GetSettingKey() => "BankAccount_CreateBankAccount";

        public override bool IsValid()
        {
            if (string.IsNullOrEmpty(BankName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(BankBranch))
            {
                return false;
            }

            if (string.IsNullOrEmpty(BankAddress))
            {
                return false;
            }

            if (string.IsNullOrEmpty(BankSwiftCode))
            {
                return false;
            }

            if (string.IsNullOrEmpty(AccountName))
            {
                return false;
            }

            if (string.IsNullOrEmpty(AccountNumber))
            {
                return false;
            }

            if (string.IsNullOrEmpty(BeneficiaryName))
            {
                return false;
            }

            return true;
        }
    }
}
