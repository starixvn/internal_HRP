namespace HRP.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Models.Bank;

    public interface IBankAccountService
    {
        CommandResult CreateBankAccount(IBankAccount bankAccount);
        IEnumerable<BankAccount> GetBankAccountsByEmployeeId(int employeeId);
    }
}
