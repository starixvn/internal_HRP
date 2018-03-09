namespace HRP.Models.BankAccount
{
    using HRP.Core.Enumerations;

    public class BankAccount
    {
        public int BankAccountId { get; set; }

        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public int CompanyId { get; set; }

        public int BankType { get; set; } = (int)BankTypes.Ebanking;

        public string BankTypeName { get; set; }

        public string BankName { get; set; }

        public string BankBranch { get; set; }

        public string BankAddress { get; set; }

        public string BankSwiftCode { get; set; }

        public string AccountName { get; set; }

        public string AccountNumber { get; set; }

        public int AccountType { get; set; } = (int)BankAccountType.Savings;

        public string AccountTypeName { get; set; }

        public string BeneficiaryName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}