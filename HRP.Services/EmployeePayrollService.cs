namespace HRP.Services
{
    using System;
    using System.Linq;
    using Fanex.Data.Repository;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models.Currency;
    using HRP.Models.EmployeeManagement;
    using HRP.Services.Interfaces;

    public class EmployeePayrollService : IEmployeePayrollService
    {
        private readonly IBaseService baseService;
        private readonly IDynamicRepository repository;

        public EmployeePayrollService(IBaseService baseService)
        {
            this.baseService = baseService;
            this.repository = new DynamicRepository();
        }

        public EmployeePayrollViewModel GetEmployeePayroll(int employeeId)
        {
            var viewModel = new EmployeePayrollViewModel
            {
                CurrencyExchangeRates = baseService.GetData<CurrencyExchangeRateModel>(new GetExchangeRateCriteria { Month = DateTime.Today.Month, Year = DateTime.Today.Year }),
                Currencies = baseService.GetData<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria()),
            };

            var criteria = new GetEmployeePayrollCriteria { EmployeeId = employeeId };
            var mappingTypes = new[] { typeof(EmployeePayrollBasicSalaryModel), typeof(EmployeePayrollAllowanceModel), typeof(EmployeePayrollAllowanceDetailModel), typeof(EmployeePayrollDeduction) };
            var result = repository.FetchMultiple(criteria, mappingTypes);

            viewModel.BasicSalary = result[0].Cast<EmployeePayrollBasicSalaryModel>().FirstOrDefault() ?? new EmployeePayrollBasicSalaryModel { EmployeeId = employeeId };
            viewModel.Allowance = result[1].Cast<EmployeePayrollAllowanceModel>().FirstOrDefault() ?? new EmployeePayrollAllowanceModel { EmployeeId = employeeId };
            viewModel.AllowanceDetails = result[2].Cast<EmployeePayrollAllowanceDetailModel>();
            viewModel.Deduction = result[3].Cast<EmployeePayrollDeduction>().FirstOrDefault() ?? new EmployeePayrollDeduction { EmployeeId = employeeId };

            return viewModel;
        }
    }
}