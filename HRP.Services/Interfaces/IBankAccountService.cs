namespace HRP.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Models.Bank;

    public interface IBankAccountService
    {
        CommandResult CreateBankAccount(IBankAccount bankAccount);

        EmployeeBankAccounts GetBankAccountsByEmployeeId(int employeeId);

        CommandResult UpdateBankAccount(IBankAccount bankAccount);

        BankAccount GetBankAccountById(int bankAccountId);

        IEnumerable<BankAccount> FilterBankAccounts(
            IEnumerable<BankAccount> bankAccounts,
            int[] bankTypes,
            bool? isActive);
    }
}