namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Services;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.MonthlyPayroll.EmployeeMonthlyPayroll;

    public class MonthlyPayrollService : IMonthlyPayrollService
    {
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;

        public MonthlyPayrollService(
            ISessionService sessionService,
            IBaseService baseService,
            ICryptoService cryptoService)
        {
            this.sessionService = sessionService;
            this.baseService = baseService;
            this.cryptoService = cryptoService;
        }

        public async Task<MonthlyPayrollFilterInfoModel> GetFilterInfoAsync(int companyId, DateTime month, int? payrollId = null)
        {
            var criteria = new GetFilterInfoCriteria
            {
                UserId = sessionService.UserId,
                CompanyId = companyId,
                Month = month,
                PayrollId = payrollId
            };

            return await baseService.GetDataEntityAsync<MonthlyPayrollFilterInfoModel>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<MultiSelectViewModel>> GetHierarchyCompanyAsync(int companyId, string costCenterIds)
        {
            var criteria = new GetHierarchyCompanyCriteria
            {
                UserId = sessionService.UserId,
                CompanyId = companyId,
                CostCenterIds = costCenterIds
            };

            Tuple<IEnumerable<MultiSelectViewModel>, IEnumerable<MultiSelectViewModel>, IEnumerable<MultiSelectViewModel>> tuple =
                await baseService.FetchMultipleAsync<MultiSelectViewModel, MultiSelectViewModel, MultiSelectViewModel>(criteria).ConfigureAwait(false);

            IEnumerable<MultiSelectViewModel> hierarchy = tuple.Item1.Select(division => new MultiSelectViewModel
            {
                Value = division.Value,
                Text = division.Text,
                Checked = division.Checked,
                Items = tuple.Item2.Where(dept => dept.ParentId == division.Value).Select(dept => new MultiSelectViewModel
                {
                    Value = dept.Value,
                    Text = dept.Text,
                    Checked = dept.Checked,
                    Items = tuple.Item3.Where(costcenter => costcenter.ParentId == dept.Value).Select(costcenter => new MultiSelectViewModel
                    {
                        Value = costcenter.Value,
                        Text = costcenter.Text,
                        Checked = costcenter.Checked
                    }).ToList()
                }).ToList()
            });

            return hierarchy;
        }

        public async Task<IEnumerable<EmployeeMonthlyPayrollModel>> GetMonthlyPayrollAsync(EmployeeMonthlyPayrollViewModel viewModel)
        {
            var criteria = new GetMonthlyPayrollCriteria
            {
                UserId = sessionService.UserId,
                Month = viewModel.Month,
                CompanyId = viewModel.CompanyId,
                CostCenterIds = viewModel.CostCenterIds,
                EmployeeId = viewModel.EmployeeId,
                Employee = viewModel.Employee,
                IDNo = viewModel.IDNo,
                WorkingDays = viewModel.WorkingDays
            };

            Tuple<IEnumerable<EmployeeMonthlyPayrollModel>,
                IEnumerable<EmployeeMonthlyPayrollBasicSalaryModel>,
                IEnumerable<EmployeeMonthlyPayrollBasicSalaryModel>> tuple = await baseService.FetchMultipleAsync<
                    EmployeeMonthlyPayrollModel,
                    EmployeeMonthlyPayrollBasicSalaryModel,
                    EmployeeMonthlyPayrollBasicSalaryModel>(criteria).ConfigureAwait(false);

            IEnumerable<EmployeeMonthlyPayrollModel> monthlyPayrolls = tuple.Item1;
            foreach (EmployeeMonthlyPayrollModel monthlyPayroll in monthlyPayrolls)
            {
                monthlyPayroll.BasicSalary = tuple.Item2.FirstOrDefault(x => x.PayrollId == monthlyPayroll.PayrollId) ?? new EmployeeMonthlyPayrollBasicSalaryModel();
                monthlyPayroll.Allowance = tuple.Item3.FirstOrDefault(x => x.PayrollId == monthlyPayroll.PayrollId) ?? new EmployeeMonthlyPayrollBasicSalaryModel();
            }

            DecryptAmount(monthlyPayrolls);
            FormatAmount(monthlyPayrolls);

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

        public Task<IEnumerable<EmployeeSuggestionViewModel>> GetEmployeeSuggestionByIDNoAsync(int companyId, string idNo)
        {
            var criteria = new GetEmployeeSuggestionByIDNoCriteria
            {
                UserId = sessionService.UserId,
                CompanyId = companyId,
                IDNo = idNo
            };

            return baseService.GetDataAsync<EmployeeSuggestionViewModel>(criteria);
        }

        private void DecryptAmount(IEnumerable<EmployeeMonthlyPayrollModel> monthlyPayrolls)
        {
            foreach (EmployeeMonthlyPayrollModel monthlyPayroll in monthlyPayrolls)
            {
                monthlyPayroll.BasicSalary.EbankingAmount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.EbankingAmount);
                monthlyPayroll.BasicSalary.DirectBanking1Amount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.DirectBanking1Amount);
                monthlyPayroll.BasicSalary.DirectBanking2Amount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.DirectBanking2Amount);
                monthlyPayroll.BasicSalary.CashAmount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.CashAmount);

                monthlyPayroll.Allowance.DirectBanking1Amount = cryptoService.Decrypt(monthlyPayroll.Allowance.DirectBanking1Amount);
                monthlyPayroll.Allowance.DirectBanking2Amount = cryptoService.Decrypt(monthlyPayroll.Allowance.DirectBanking2Amount);
                monthlyPayroll.Allowance.CashAmount = cryptoService.Decrypt(monthlyPayroll.Allowance.CashAmount);
            }
        }

        private static void FormatAmount(IEnumerable<EmployeeMonthlyPayrollModel> monthlyPayrolls)
        {
            foreach (EmployeeMonthlyPayrollModel monthlyPayroll in monthlyPayrolls)
            {
                monthlyPayroll.BasicSalary.EbankingAmount = CalculationService.FormatAmount(
                    monthlyPayroll.BasicSalary.EbankingAmount,
                    monthlyPayroll.BasicSalary.EbankingCurrencyCode);
                monthlyPayroll.BasicSalary.DirectBanking1Amount = CalculationService.FormatAmount(
                    monthlyPayroll.BasicSalary.DirectBanking1Amount,
                    monthlyPayroll.BasicSalary.DirectBanking1CurrencyCode);
                monthlyPayroll.BasicSalary.DirectBanking2Amount = CalculationService.FormatAmount(
                    monthlyPayroll.BasicSalary.DirectBanking2Amount,
                    monthlyPayroll.BasicSalary.DirectBanking2CurrencyCode);
                monthlyPayroll.BasicSalary.CashAmount = CalculationService.FormatAmount(
                    monthlyPayroll.BasicSalary.CashAmount,
                    monthlyPayroll.BasicSalary.CashCurrencyCode);

                monthlyPayroll.Allowance.DirectBanking1Amount = CalculationService.FormatAmount(
                    monthlyPayroll.Allowance.DirectBanking1Amount,
                    monthlyPayroll.Allowance.DirectBanking1CurrencyCode);
                monthlyPayroll.Allowance.DirectBanking2Amount = CalculationService.FormatAmount(
                    monthlyPayroll.Allowance.DirectBanking2Amount,
                    monthlyPayroll.Allowance.DirectBanking2CurrencyCode);
                monthlyPayroll.Allowance.CashAmount = CalculationService.FormatAmount(
                    monthlyPayroll.Allowance.CashAmount,
                    monthlyPayroll.Allowance.CashCurrencyCode);
            }
        }
    }
}