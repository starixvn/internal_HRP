namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Model;
    using Core.Services;
    using Criteria.BankAccount;
    using Models.BankAccount;

    public class BankAccountService(IBaseService baseService) : IBankAccountService
    {
        public async Task<IList<BankAccount>> GetBankAccountsByEmployeeIdAsync(int employeeId, int? bankTypeId, bool? isActive)
        {
            var criteria = new GetBankAccountsByEmployeeIdCriteria
            {
                EmployeeId = employeeId,
                BankTypeId = bankTypeId,
                IsActive = isActive
            };

            return (await baseService.GetDataAsync<BankAccount>(criteria)).ToList();
        }

        public async Task<BankAccount> GetBankAccountByIdAsync(int bankAccountId)
        {
            var criteria = new GetBankAccountByIdCriteria { Id = bankAccountId };
            return await baseService.GetDataEntityAsync<BankAccount>(criteria);
        }

        public async Task<CommandResult> CreateBankAccountAsync(int actorId, CreateBankAccountViewModel bankAccount)
        {
            var criteria = new CreateBankAccountCriteria
            {
                EmployeeId = bankAccount.EmployeeId,
                BankType = bankAccount.BankTypeId,
                BankName = bankAccount.BankName,
                BankBranch = bankAccount.BankBranch,
                BankAddress = bankAccount.BankAddress,
                BankSwiftCode = bankAccount.BankSwiftCode,
                PaymentTransactionId = bankAccount.PaymentTransactionId,
                AccountNumber = bankAccount.AccountNumber,
                AccountType = bankAccount.AccountTypeId,
                BeneficiaryName = bankAccount.BeneficiaryName,
                IsActive = bankAccount.IsActive,
                CreatedBy = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public async Task<CommandResult> EditBankAccountAsync(int actorId, EditBankAccountViewModel bankAccount)
        {
            var criteria = new EditBankAccountCriteria
            {
                Id = bankAccount.BankAccountId,
                EmployeeId = bankAccount.EmployeeId,
                BankType = bankAccount.BankTypeId,
                BankName = bankAccount.BankName,
                BankBranch = bankAccount.BankBranch,
                BankAddress = bankAccount.BankAddress,
                BankSwiftCode = bankAccount.BankSwiftCode,
                PaymentTransactionId = bankAccount.PaymentTransactionId,
                AccountNumber = bankAccount.AccountNumber,
                AccountType = bankAccount.AccountTypeId,
                BeneficiaryName = bankAccount.BeneficiaryName,
                IsActive = bankAccount.IsActive,
                UpdatedBy = actorId
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
        }
    }
}