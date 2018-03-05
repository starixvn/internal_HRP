namespace HRP.Services
{
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models;
    using HRP.Repositories;

    public class MonthlyPayrollService
    {
        private readonly ISessionService sessionService;
        private readonly IMonthlyPayrollRepository monthlyPayrollRepository;

        public MonthlyPayrollService(
            ISessionService sessionService,
            IMonthlyPayrollRepository monthlyPayrollRepository = null)
        {
            this.sessionService = sessionService;
            this.monthlyPayrollRepository = monthlyPayrollRepository ?? new MonthlyPayrollRepository();
        }

        public async Task<EmployeeMonthlyPayrollViewModel> GetDataFilterForIndexPageAsync(int companyId, string costCenterIds)
        {
            var criteria = new GetDataFilterForIndexPageCriteria
            {
                UserId = sessionService.UserId,
                CompanyId = companyId,
                CostCenterIds = costCenterIds
            };
            var viewModel = new EmployeeMonthlyPayrollViewModel();

            if (criteria.IsValid())
            {
                viewModel = await monthlyPayrollRepository.GetDataFilterForIndexPageAsync(criteria).ConfigureAwait(false);
            }

            return viewModel;
        }
    }
}