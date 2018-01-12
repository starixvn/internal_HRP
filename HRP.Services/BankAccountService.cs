namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Model;
    using Core.Services;
    using Criteria.Bank;
    using Interfaces;
    using Models.Bank;

    public class BankAccountService : BaseService, IBankAccountService
    {
        private readonly IBaseService baseService;

        public BankAccountService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public CommandResult CreateBankAccount(IBankAccount bankAccount)
        {
            var criteria = new CreateBankAccountCriteria(bankAccount);
            return baseService.ExecuteNonQuery(criteria);
        }

        public IEnumerable<BankAccount> GetBankAccountsByEmployeeId(int employeeId)
        {
            var criteria = new GetBankAccountsByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            return baseService.GetData<BankAccount>(criteria);
        }

        public IEnumerable<BankAccount> FilterBankAccounts(IEnumerable<BankAccount> bankAccounts, int[] bankTypes, bool? isActive)
        {
            var filteredBankAccounts = bankAccounts;
            if (isActive != null)
            {
                filteredBankAccounts = filteredBankAccounts.Where(x => x.IsActive = isActive.Value);
            }

            if (!bankTypes.Contains(0))
            {
                filteredBankAccounts = filteredBankAccounts.Where(x => bankTypes.Contains(x.BankType));
            }

            return filteredBankAccounts;
        }

        public IEnumerable<BankType> GetBankTypes()
        {
            return baseService.GetData<BankType>(new GetBankTypesCriteria());
        }

        public IEnumerable<AccountType> GetAccountTypes()
        {
            return baseService.GetData<AccountType>(new GetAccountTypesCriteria());
        }
    }
}
