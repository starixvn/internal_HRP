namespace HRP.Models.Bank
{
    public class BankAccount : IBankAccount
    {
        public BankAccount()
        {
            BankType = 1;
            AccountType = 1;
            IsActive = true;
        }

        public int Id { get; set; }

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
    }
}
