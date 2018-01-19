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
        private readonly ICertificateService certificateService;

        public EmployeePayrollService(IBaseService baseService, IDynamicRepository repository, ICertificateService certificateService)
        {
            this.baseService = baseService;
            this.repository = repository;
            this.certificateService = certificateService;
        }

        public EmployeePayrollViewModel GetEmployeePayroll(int employeeId, int userId)
        {
            var criteria = new GetEmployeePayrollCriteria { EmployeeId = employeeId, UserId = userId };
            var mappingTypes = new[] { typeof(EmployeePayrollViewModel), typeof(EmployeePayrollBasicSalaryModel), typeof(EmployeePayrollAllowanceModel), typeof(EmployeePayrollDeduction) };
            var result = repository.FetchMultiple(criteria, mappingTypes);

            var viewModel = result[0].Cast<EmployeePayrollViewModel>().FirstOrDefault();

            if (viewModel != null)
            {
                viewModel.CanRead = criteria.CanRead;
                viewModel.CanUpdate = criteria.CanUpdate;

                viewModel.BasicSalary = result[1].Cast<EmployeePayrollBasicSalaryModel>().FirstOrDefault()
                    ?? new EmployeePayrollBasicSalaryModel { EmployeeId = employeeId, WorkingDays = 22, AutoCalculateOn = "Cash" };
                viewModel.Allowance = result[2].Cast<EmployeePayrollAllowanceModel>().FirstOrDefault() ?? new EmployeePayrollAllowanceModel { EmployeeId = employeeId, AutoCalculateOn = "Cash" };
                viewModel.Deduction = result[3].Cast<EmployeePayrollDeduction>().FirstOrDefault() ?? new EmployeePayrollDeduction { EmployeeId = employeeId };
                viewModel.CurrencyExchangeRates = baseService.GetData<CurrencyExchangeRateModel>(new GetExchangeRateCriteria { Month = DateTime.Today.Month, Year = DateTime.Today.Year });
                viewModel.Currencies = baseService.GetData<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria());
                viewModel.ChangeReasons = baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.PayrollSalaryChangeReason });

                DecryptData(viewModel);
            }

            return viewModel;
        }

        private void DecryptData(EmployeePayrollViewModel viewModel)
        {
            viewModel.BasicSalary.Salary = certificateService.Decrypt(viewModel.BasicSalary.Salary);
            viewModel.BasicSalary.EbankingAmount = certificateService.Decrypt(viewModel.BasicSalary.EbankingAmount);
            viewModel.BasicSalary.DirectBanking1Amount = certificateService.Decrypt(viewModel.BasicSalary.DirectBanking1Amount);
            viewModel.BasicSalary.DirectBanking2Amount = certificateService.Decrypt(viewModel.BasicSalary.DirectBanking2Amount);
            viewModel.BasicSalary.CashAmount = certificateService.Decrypt(viewModel.BasicSalary.CashAmount);

            viewModel.Allowance.DirectBanking1Amount = certificateService.Decrypt(viewModel.Allowance.DirectBanking1Amount);
            viewModel.Allowance.DirectBanking2Amount = certificateService.Decrypt(viewModel.Allowance.DirectBanking2Amount);
            viewModel.Allowance.CashAmount = certificateService.Decrypt(viewModel.Allowance.CashAmount);
            viewModel.Allowance.SubsistenceAmount = certificateService.Decrypt(viewModel.Allowance.SubsistenceAmount);
            viewModel.Allowance.HomePassageAmount = certificateService.Decrypt(viewModel.Allowance.HomePassageAmount);
            viewModel.Allowance.HousingAmount = certificateService.Decrypt(viewModel.Allowance.HousingAmount);
            viewModel.Allowance.MobilePhoneAmount = certificateService.Decrypt(viewModel.Allowance.MobilePhoneAmount);
            viewModel.Allowance.HPIAmount = certificateService.Decrypt(viewModel.Allowance.HPIAmount);
            viewModel.Allowance.DutyMealAmount = certificateService.Decrypt(viewModel.Allowance.DutyMealAmount);
            viewModel.Allowance.SecondmentAmount = certificateService.Decrypt(viewModel.Allowance.SecondmentAmount);
            viewModel.Allowance.OthersAmount = certificateService.Decrypt(viewModel.Allowance.OthersAmount);

            viewModel.Deduction.RoomingChargesAmount = certificateService.Decrypt(viewModel.Deduction.RoomingChargesAmount);
            viewModel.Deduction.FitnessFirstAmount = certificateService.Decrypt(viewModel.Deduction.FitnessFirstAmount);
        }

        public void UpdatePayroll(EmployeePayrollViewModel model, int userId)
        {
            var criteria = new UpdateEmployeePayrollCriteria
            {
                EmployeeId = model.EmployeeId,
                Salary = certificateService.Encrypt(model.BasicSalary.Salary),
                CurrencyCode = model.BasicSalary.CurrencyCode,
                WorkingDays = model.BasicSalary.WorkingDays,
                EbankingCurrencyCode = model.BasicSalary.EbankingCurrencyCode,
                EbankingAmount = certificateService.Encrypt(model.BasicSalary.EbankingAmount),
                EbankingAccountId = model.BasicSalary.EbankingAccountId,
                DirectBanking1CurrencyCode = model.BasicSalary.DirectBanking1CurrencyCode,
                DirectBanking1Amount = certificateService.Encrypt(model.BasicSalary.DirectBanking1Amount),
                DirectBanking1AccountId = model.BasicSalary.DirectBanking1AccountId,
                DirectBanking1BankTypeId = model.BasicSalary.DirectBanking1BankTypeId,
                DirectBanking2CurrencyCode = model.BasicSalary.DirectBanking2CurrencyCode,
                DirectBanking2Amount = certificateService.Encrypt(model.BasicSalary.DirectBanking2Amount),
                DirectBanking2AccountId = model.BasicSalary.DirectBanking2AccountId,
                DirectBanking2BankTypeId = model.BasicSalary.DirectBanking2BankTypeId,
                CashCurrencyCode = model.BasicSalary.CashCurrencyCode,
                CashAmount = certificateService.Encrypt(model.BasicSalary.CashAmount),
                CashAccountId = model.BasicSalary.CashAccountId,
                AutoCalculateOn = model.BasicSalary.AutoCalculateOn,
                EffectiveDate = model.BasicSalary.EffectiveDate,
                ReasonId = model.BasicSalary.ReasonId,
                AllowanceCurrencyCode = model.Allowance.CurrencyCode,
                AllowanceDirectBanking1CurrencyCode = model.Allowance.DirectBanking1CurrencyCode,
                AllowanceDirectBanking1Amount = certificateService.Encrypt(model.Allowance.DirectBanking1Amount),
                AllowanceDirectBanking1AccountId = model.Allowance.DirectBanking1AccountId,
                AllowanceDirectBanking1BankTypeId = model.Allowance.DirectBanking1BankTypeId,
                AllowanceDirectBanking2CurrencyCode = model.Allowance.DirectBanking2CurrencyCode,
                AllowanceDirectBanking2Amount = certificateService.Encrypt(model.Allowance.DirectBanking2Amount),
                AllowanceDirectBanking2AccountId = model.Allowance.DirectBanking2AccountId,
                AllowanceDirectBanking2BankTypeId = model.Allowance.DirectBanking2BankTypeId,
                AllowanceCashCurrencyCode = model.Allowance.CashCurrencyCode,
                AllowanceCashAmount = certificateService.Encrypt(model.Allowance.CashAmount),
                AllowanceCashAccountId = model.Allowance.CashAccountId,
                AllowanceAutoCalculateOn = model.Allowance.AutoCalculateOn,
                SubsistenceCurrencyCode = model.Allowance.SubsistenceCurrencyCode,
                SubsistenceAmount = certificateService.Encrypt(model.Allowance.SubsistenceAmount),
                HomePassageCurrencyCode = model.Allowance.HomePassageCurrencyCode,
                HomePassageAmount = certificateService.Encrypt(model.Allowance.HomePassageAmount),
                HousingCurrencyCode = model.Allowance.HousingCurrencyCode,
                HousingAmount = certificateService.Encrypt(model.Allowance.HousingAmount),
                MobilePhoneCurrencyCode = model.Allowance.MobilePhoneCurrencyCode,
                MobilePhoneAmount = certificateService.Encrypt(model.Allowance.MobilePhoneAmount),
                HPICurrencyCode = model.Allowance.HPICurrencyCode,
                HPIAmount = certificateService.Encrypt(model.Allowance.HPIAmount),
                DutyMealCurrencyCode = model.Allowance.DutyMealCurrencyCode,
                DutyMealAmount = certificateService.Encrypt(model.Allowance.DutyMealAmount),
                SecondmentCurrencyCode = model.Allowance.SecondmentCurrencyCode,
                SecondmentAmount = certificateService.Encrypt(model.Allowance.SecondmentAmount),
                OthersCurrencyCode = model.Allowance.OthersCurrencyCode,
                OthersAmount = certificateService.Encrypt(model.Allowance.OthersAmount),
                RoomingChargesCurrencyCode = model.Deduction.RoomingChargesCurrencyCode,
                RoomingChargesAmount = certificateService.Encrypt(model.Deduction.RoomingChargesAmount),
                FitnessFirstCurrencyCode = model.Deduction.FitnessFirstCurrencyCode,
                FitnessFirstAmount = certificateService.Encrypt(model.Deduction.FitnessFirstAmount),
                UpdatedBy = userId
            };

            baseService.ExecuteNonQuery(criteria);
        }
    }
}