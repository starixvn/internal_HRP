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
            var criteria = new GetEmployeePayrollCriteria { EmployeeId = employeeId };
            var mappingTypes = new[] { typeof(EmployeePayrollViewModel), typeof(EmployeePayrollBasicSalaryModel), typeof(EmployeePayrollAllowanceModel), typeof(EmployeePayrollDeduction) };
            var result = repository.FetchMultiple(criteria, mappingTypes);

            var viewModel = result[0].Cast<EmployeePayrollViewModel>().FirstOrDefault();

            if(viewModel != null)
            {
                viewModel.BasicSalary = result[1].Cast<EmployeePayrollBasicSalaryModel>().FirstOrDefault() 
                    ?? new EmployeePayrollBasicSalaryModel { EmployeeId = employeeId, WorkingDays = 22, AutoCalculateOn = "Cash" };
                viewModel.Allowance = result[2].Cast<EmployeePayrollAllowanceModel>().FirstOrDefault() ?? new EmployeePayrollAllowanceModel { EmployeeId = employeeId, AutoCalculateOn = "Cash" };
                viewModel.Deduction = result[3].Cast<EmployeePayrollDeduction>().FirstOrDefault() ?? new EmployeePayrollDeduction { EmployeeId = employeeId };
                viewModel.CurrencyExchangeRates = baseService.GetData<CurrencyExchangeRateModel>(new GetExchangeRateCriteria { Month = DateTime.Today.Month, Year = DateTime.Today.Year });
                viewModel.Currencies = baseService.GetData<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria());
                viewModel.ChangeReasons = baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.PayrollSalaryChangeReason });
            }

            return viewModel;
        }

        public void UpdatePayroll(EmployeePayrollViewModel viewModel)
        {

        }
    }
}