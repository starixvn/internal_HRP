namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;
    using HRP.Repositories;

    public class MonthlyPayrollService : IMonthlyPayrollService
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly IMonthlyPayrollRepository monthlyPayrollRepository;

        public MonthlyPayrollService(
            ISessionService sessionService,
            IBaseService baseService,
            ICryptoService cryptoService,
            IMonthlyPayrollRepository monthlyPayrollRepository = null)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
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

        public async Task<IEnumerable<MonthlyPayrollEmployeeInfoViewModel>> GetMonthlyPayrollAsync(EmployeeMonthlyPayrollViewModel viewModel)
        {
            IEnumerable<MonthlyPayrollEmployeeInfoViewModel> monthlyPayrolls = new List<MonthlyPayrollEmployeeInfoViewModel>();
            var criteria = new GetMonthlyPayrollCriteria
            {
                UserId = sessionService.UserId,
                Month = viewModel.Month,
                CompanyId = viewModel.CompanyId,
                CostCenterIds = viewModel.CostCenterIds,
                EmployeeId = viewModel.EmployeeId,
                WorkingDays = viewModel.WorkingDays
            };

            if (criteria.IsValid())
            {
                monthlyPayrolls = await monthlyPayrollRepository.GetMonthlyPayrollAsync(criteria).ConfigureAwait(false);
                DecryptMonthlyPayroll(monthlyPayrolls);
            }

            return monthlyPayrolls;
        }

        public Task<IEnumerable<EmployeeSuggestionViewModel>> GetEmployeeSuggestionByNameAsync(int companyId, string name)
        {
            var criteria = new GetEmployeeSuggestionByNameCriteria
            {
                UserId = sessionService.UserId,
                CompanyId = companyId,
                Name = name
            };

            return baseService.GetDataAsync<EmployeeSuggestionViewModel>(criteria);
        }

        public async Task<CommandResult> ResetMonthlySalaryAsync()
        {
            var criteria = new ResetMonthlySalaryCriteria();
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> UpdateMonthlySalaryAsync()
        {
            var criteria = new UpdateMonthlySalaryCriteria();
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        private void DecryptMonthlyPayroll(IEnumerable<MonthlyPayrollEmployeeInfoViewModel> monthlyPayrolls)
        {
            foreach (var monthlyPayroll in monthlyPayrolls)
            {
                monthlyPayroll.BasicSalary.EBankingAmount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.EBankingAmount);
                monthlyPayroll.BasicSalary.DirectBanking1Amount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.DirectBanking1Amount);
                monthlyPayroll.BasicSalary.DirectBanking2Amount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.DirectBanking2Amount);
                monthlyPayroll.BasicSalary.CashAmount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.CashAmount);

                monthlyPayroll.Allowance.DirectBanking1Amount = cryptoService.Decrypt(monthlyPayroll.Allowance.DirectBanking1Amount);
                monthlyPayroll.Allowance.DirectBanking2Amount = cryptoService.Decrypt(monthlyPayroll.Allowance.DirectBanking2Amount);
                monthlyPayroll.Allowance.CashAmount = cryptoService.Decrypt(monthlyPayroll.Allowance.CashAmount);
            }
        }
    }
}