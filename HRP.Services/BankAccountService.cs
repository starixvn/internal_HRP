namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services;
    using Criteria.BankAccount;
    using HRP.Core.Model;
    using Models.BankAccount;

    public class BankAccountService : IBankAccountService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public BankAccountService(
            IBaseService baseService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        public async Task<IList<BankAccount>> GetBankAccountsByEmployeeIdAsync(int employeeId, int? bankTypeId, bool? isActive)
        {
            var criteria = new GetBankAccountsByEmployeeIdCriteria
            {
                EmployeeId = employeeId,
                BankTypeId = bankTypeId,
                IsActive = isActive
            };

            return (await baseService.GetDataAsync<BankAccount>(criteria).ConfigureAwait(false)).ToList();
        }

        public async Task<BankAccount> GetBankAccountByIdAsync(int bankAccountId)
        {
            var criteria = new GetBankAccountByIdCriteria { Id = bankAccountId };
            return await baseService.GetDataEntityAsync<BankAccount>(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> CreateBankAccountAsync(CreateBankAccountViewModel bankAccount)
        {
            var criteria = new CreateBankAccountCriteria
            {
                EmployeeId = bankAccount.EmployeeId,
                BankType = bankAccount.BankTypeId,
                BankName = bankAccount.BankName,
                BankBranch = bankAccount.BankBranch,
                BankAddress = bankAccount.BankAddress,
                BankSwiftCode = bankAccount.BankSwiftCode,
                AccountName = bankAccount.AccountName,
                AccountNumber = bankAccount.AccountNumber,
                AccountType = bankAccount.AccountTypeId,
                BeneficiaryName = bankAccount.BeneficiaryName,
                IsActive = bankAccount.IsActive,
                CreatedBy = sessionService.UserId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public async Task<CommandResult> EditBankAccountAsync(EditBankAccountViewModel bankAccount)
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
                AccountName = bankAccount.AccountName,
                AccountNumber = bankAccount.AccountNumber,
                AccountType = bankAccount.AccountTypeId,
                BeneficiaryName = bankAccount.BeneficiaryName,
                IsActive = bankAccount.IsActive,
                UpdatedBy = sessionService.UserId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }
    }
}