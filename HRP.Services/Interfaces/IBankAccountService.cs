namespace HRP.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Models.Bank;

    public interface IBankAccountService
    {
        CommandResult CreateBankAccount(IBankAccount bankAccount);

        IList<BankAccount> GetBankAccountsByEmployeeId(int employeeId, int bankTypeId, bool? isActive);

        CommandResult UpdateBankAccount(IBankAccount bankAccount);

        BankAccount GetBankAccountById(int bankAccountId);

        bool CheckIsAccountNameExisted(IBankAccount bankAccount);

        bool CheckIsAccountNumberExisted(IBankAccount bankAccount);

        IEnumerable<BankAccount> FilterBankAccounts(
            IEnumerable<BankAccount> bankAccounts,
            int bankType,
            bool? isActive);
    }
}