namespace HRP.Models.Bank
{
    using System.Collections.Generic;

    public class EmployeeBankAccounts
    {
        public int EmployeeId { get; set; }

        public int CompanyId { get; set; }

        public string EmployeeName { get; set; }

        public IList<BankAccount> BankAccounts { get; set; }
    }
}