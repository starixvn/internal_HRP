namespace HRP.Models.Bank
{
    public interface IBankAccount
    {
        string AccountName { get; set; }
        string AccountNumber { get; set; }
        int AccountType { get; set; }
        string BankAddress { get; set; }
        string BankBranch { get; set; }
        string BankName { get; set; }
        string BankSwiftCode { get; set; }
        int BankType { get; set; }
        string BeneficiaryName { get; set; }
        int EmployeeId { get; set; }
        bool IsActive { get; set; }
    }
}