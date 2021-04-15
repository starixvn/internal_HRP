namespace HRP.Criteria.BankAccount
{
    public abstract class BankAccountBaseCriteria : BaseNonQueryCommand
    {
        public int PaymentTransactionId { get; set; }

        public string AccountNumber { get; set; }

        public short AccountType { get; set; }

        public string BankAddress { get; set; }

        public string BankBranch { get; set; }

        public string BankName { get; set; }

        public string BankSwiftCode { get; set; }

        public short BankType { get; set; }

        public string BeneficiaryName { get; set; }

        public int EmployeeId { get; set; }

        public bool IsActive { get; set; }

        public override bool IsValid()
        {
            if (string.IsNullOrEmpty(BankName))
            {
                return false;
            }

            if (PaymentTransactionId == 0)
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