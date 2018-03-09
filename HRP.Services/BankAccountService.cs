namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services;
    using Criteria.BankAccount;
    using Models.BankAccount;

    public class BankAccountService : IBankAccountService
    {
        private readonly IBaseService baseService;

        public BankAccountService(IBaseService baseService)
        {
            this.baseService = baseService;
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
    }
}