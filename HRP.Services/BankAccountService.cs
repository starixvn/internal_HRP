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

        public BankAccount GetBankAccountById(int bankAccountId)
        {
            var criteria = new GetBankAccountByIdCriteria { Id = bankAccountId };
            return baseService.GetDataEntity<BankAccount>(criteria);
        }

        ////public CommandResult EditBankAccount(BankAccount bankAccount)
        ////{

        ////}
        
        public IEnumerable<BankAccount> FilterBankAccounts(IEnumerable<BankAccount> bankAccounts, int[] bankTypes, bool[] isActive)
        {
            var filteredBankAccounts = bankAccounts;
            if (isActive != null && isActive.Any())
            {
                filteredBankAccounts = filteredBankAccounts.Where(x => isActive.Contains(x.IsActive));
            }

            if (bankTypes != null && !bankTypes.Contains(0))
            {
                filteredBankAccounts = filteredBankAccounts.Where(x => bankTypes.Contains(x.BankType));
            }

            return filteredBankAccounts;
        }
    }
}
