namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
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

        public IList<BankAccount> GetBankAccountsByEmployeeId(int employeeId, int? bankTypeId, bool? isActive)
        {
            var criteria = new GetBankAccountsByEmployeeIdCriteria
            {
                EmployeeId = employeeId,
                BankTypeId = bankTypeId,
                IsActive = isActive
            };

            return baseService.GetData<BankAccount>(criteria).ToList();
        }

        public BankAccount GetBankAccountById(int bankAccountId)
        {
            var criteria = new GetBankAccountByIdCriteria { Id = bankAccountId };
            return baseService.GetDataEntity<BankAccount>(criteria);
        }
    }
}