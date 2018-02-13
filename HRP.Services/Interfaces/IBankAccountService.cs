namespace HRP.Services.Interfaces
{
    using System.Collections.Generic;
    using Models.BankAccount;

    public interface IBankAccountService
    {
        IList<BankAccount> GetBankAccountsByEmployeeId(int employeeId, int? bankTypeId, bool? isActive);

        BankAccount GetBankAccountById(int bankAccountId);
    }
}