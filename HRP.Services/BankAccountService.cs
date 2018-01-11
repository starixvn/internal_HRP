namespace HRP.Services
{
    using System.Collections.Generic;
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
            var criteria = bankAccount as CreateBankAccountCriteria;
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
