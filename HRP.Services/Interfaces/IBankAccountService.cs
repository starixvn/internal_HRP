namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.BankAccount;

    public interface IBankAccountService
    {
        Task<IList<BankAccount>> GetBankAccountsByEmployeeIdAsync(int employeeId, int? bankTypeId, bool? isActive);

        Task<BankAccount> GetBankAccountByIdAsync(int bankAccountId);
    }
}