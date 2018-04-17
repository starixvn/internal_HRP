namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Core.Model;
    using Models.BankAccount;

    public interface IBankAccountService
    {
        Task<IList<BankAccount>> GetBankAccountsByEmployeeIdAsync(int employeeId, int? bankTypeId, bool? isActive);

        Task<BankAccount> GetBankAccountByIdAsync(int bankAccountId);

        Task<CommandResult> CreateBankAccountAsync(CreateBankAccountViewModel bankAccount);

        Task<CommandResult> EditBankAccountAsync(EditBankAccountViewModel bankAccount);
    }
}