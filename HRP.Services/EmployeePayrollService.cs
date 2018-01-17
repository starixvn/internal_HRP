namespace HRP.Services
{
    using System;
    using System.Linq;
    using Fanex.Data.Repository;
    using HRP.Core.Constants;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.EmployeeManagement;
    using HRP.Services.Interfaces;

    public class EmployeePayrollService : IEmployeePayrollService
    {
        private readonly IBaseService baseService;
        private readonly IDynamicRepository repository;

        public EmployeePayrollService(IBaseService baseService, IDynamicRepository repository)
        {
            this.baseService = baseService;
            this.repository = repository;
        }

        public EmployeePayrollViewModel GetEmployeePayroll(int employeeId)
        {
            var viewModel = new EmployeePayrollViewModel
            {
                EmployeeId = employeeId,
                CurrencyExchangeRates = baseService.GetData<CurrencyExchangeRateModel>(new GetExchangeRateCriteria { Month = DateTime.Today.Month, Year = DateTime.Today.Year }),
                Currencies = baseService.GetData<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria()),
                ChangeReasons = baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.PayrollSalaryChangeReason })
            };

            var criteria = new GetEmployeePayrollCriteria { EmployeeId = employeeId };
            var mappingTypes = new[] { typeof(EmployeePayrollBasicSalaryModel), typeof(EmployeePayrollAllowanceModel), typeof(EmployeePayrollDeduction) };
            var result = repository.FetchMultiple(criteria, mappingTypes);

            viewModel.BasicSalary = result[0].Cast<EmployeePayrollBasicSalaryModel>().FirstOrDefault() 
                ?? new EmployeePayrollBasicSalaryModel { EmployeeId = employeeId, WorkingDays = 22, AutoCalculateOn = "Cash" };
            viewModel.Allowance = result[1].Cast<EmployeePayrollAllowanceModel>().FirstOrDefault() ?? new EmployeePayrollAllowanceModel { EmployeeId = employeeId, AutoCalculateOn = "Cash" };
            viewModel.Deduction = result[2].Cast<EmployeePayrollDeduction>().FirstOrDefault() ?? new EmployeePayrollDeduction { EmployeeId = employeeId };

            return viewModel;
        }
    }
}