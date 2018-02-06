namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Model;
    using Core.Services;
    using Criteria.Bank;
    using Fanex.Data;
    using Interfaces;
    using Models.Bank;

    public class BankAccountService : IBankAccountService
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

        public bool CheckIsBankAccountExisted(IBankAccount bankAccount)
        {
            var criteria = new CheckBankAccountExistedCriteria
            {
                Id = bankAccount.Id,
                AccountName = bankAccount.AccountName,
                AccountNumber = bankAccount.AccountNumber,
                EmployeeId = bankAccount.EmployeeId
            };

            return baseService.GetDataEntity<bool>(criteria);
        }

        public EmployeeBankAccounts GetBankAccountsByEmployeeId(int employeeId)
        {
            var criteria = new GetBankAccountsByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var results = db.QueryMultiple(criteria))
                {
                    var viewModel = new EmployeeBankAccounts
                    {
                        BankAccounts = results.Read<BankAccount>().ToList(),
                        CompanyId = results.ReadSingle<int>(),
                        EmployeeName = results.ReadSingle<string>()
                    };

                    return viewModel;
                }
            }
        }

        public BankAccount GetBankAccountById(int bankAccountId)
        {
            var criteria = new GetBankAccountByIdCriteria { Id = bankAccountId };
            return baseService.GetDataEntity<BankAccount>(criteria);
        }

        public CommandResult UpdateBankAccount(IBankAccount bankAccount)
        {
            var criteria = new UpdateBankAccountCriteria(bankAccount);
            return baseService.ExecuteNonQuery(criteria);
        }

        public IEnumerable<BankAccount> FilterBankAccounts(IEnumerable<BankAccount> bankAccounts, int bankType, bool? isActive)
        {
            var filteredBankAccounts = bankAccounts.ToList();
            if (isActive != null)
            {
                filteredBankAccounts = filteredBankAccounts.Where(x => x.IsActive == isActive.Value).ToList();
            }

            if (bankType != 0)
            {
                filteredBankAccounts = filteredBankAccounts.Where(x => x.BankType == bankType).ToList();
            }

            //// Left here just in case spec change to multiple banktypes selection
            ////if (bankTypes != null && !bankTypes.Contains(0))
            ////{
            ////    filteredBankAccounts = filteredBankAccounts.Where(x => bankTypes.Contains(x.BankType));
            ////}

            return filteredBankAccounts.OrderBy(x => x.AccountName);
        }
    }
}