namespace HRP.Models.BankAccount
{
    using Core.Enumerations;

    public class BankAccount
    {
        public int BankAccountId { get; set; }

        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public int CompanyId { get; set; }

        public short BankType { get; set; } = (short)BankTypes.Ebanking;

        public string BankTypeName { get; set; }

        public string BankName { get; set; }

        public string BankBranch { get; set; }

        public string BankAddress { get; set; }

        public string BankSwiftCode { get; set; }

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public short AccountType { get; set; } = (short)BankAccountType.Savings;

        public string AccountTypeName { get; set; }

        public string BeneficiaryName { get; set; }

        public int PaymentTransactionId { get; set; }

        public string PaymentTransaction { get; set; }

        public bool IsActive { get; set; } = true;
    }
}