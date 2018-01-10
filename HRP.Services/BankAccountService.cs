namespace HRP.Services
{
    using System.Threading.Tasks;
    using Core.Model;
    using Core.Services;
    using Criteria.Bank;

    public class BankAccountService : BaseService
    {
        ////private readonly IBaseService baseService;

        public BankAccountService(IBaseService baseService)
        {
            ////this.baseService = baseService;
        }

        ////public async Task<CommandResult> CreateBankAccount(CreateBankAccountCriteria criteria)
        ////{
        ////    var result = baseService.ExecuteNonQuery(criteria);
        ////}
    }
}
