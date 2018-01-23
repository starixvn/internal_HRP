namespace HRP.Services
{
    using System.Linq;
    using Fanex.Data.Repository;
    using HRP.Core.Constants;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.EmployeeManagement;

    public class EmployeePayrollService : IEmployeePayrollService
    {
        private readonly IBaseService baseService;
        private readonly IDynamicRepository repository;
        private readonly ICryptoService cryptoService;

        public EmployeePayrollService(IBaseService baseService, IDynamicRepository repository, ICryptoService cryptoService)
        {
            this.baseService = baseService;
            this.repository = repository;
            this.cryptoService = cryptoService;
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
                viewModel.CurrencyExchangeRates = baseService.GetData<CurrencyExchangeRateModel>(new GetLatestExchangeRatesCriteria());
                viewModel.Currencies = baseService.GetData<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria());
                viewModel.ChangeReasons = baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.PayrollSalaryChangeReason });
                viewModel.CashAccounts = baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.CashAccount });

                DecryptData(viewModel);
            }

            return viewModel;
        }

        private void DecryptData(EmployeePayrollViewModel viewModel)
        {
            viewModel.BasicSalary.Salary = viewModel.BasicSalary.OldSalary = cryptoService.Decrypt(viewModel.BasicSalary.Salary);
            viewModel.BasicSalary.EbankingAmount = cryptoService.Decrypt(viewModel.BasicSalary.EbankingAmount);
            viewModel.BasicSalary.DirectBanking1Amount = cryptoService.Decrypt(viewModel.BasicSalary.DirectBanking1Amount);
            viewModel.BasicSalary.DirectBanking2Amount = cryptoService.Decrypt(viewModel.BasicSalary.DirectBanking2Amount);
            viewModel.BasicSalary.CashAmount = cryptoService.Decrypt(viewModel.BasicSalary.CashAmount);

            viewModel.Allowance.DirectBanking1Amount = cryptoService.Decrypt(viewModel.Allowance.DirectBanking1Amount);
            viewModel.Allowance.DirectBanking2Amount = cryptoService.Decrypt(viewModel.Allowance.DirectBanking2Amount);
            viewModel.Allowance.CashAmount = cryptoService.Decrypt(viewModel.Allowance.CashAmount);
            viewModel.Allowance.SubsistenceAmount = cryptoService.Decrypt(viewModel.Allowance.SubsistenceAmount);
            viewModel.Allowance.HomePassageAmount = cryptoService.Decrypt(viewModel.Allowance.HomePassageAmount);
            viewModel.Allowance.HousingAmount = cryptoService.Decrypt(viewModel.Allowance.HousingAmount);
            viewModel.Allowance.MobilePhoneAmount = cryptoService.Decrypt(viewModel.Allowance.MobilePhoneAmount);
            viewModel.Allowance.HPIAmount = cryptoService.Decrypt(viewModel.Allowance.HPIAmount);
            viewModel.Allowance.DutyMealAmount = cryptoService.Decrypt(viewModel.Allowance.DutyMealAmount);
            viewModel.Allowance.SecondmentAmount = cryptoService.Decrypt(viewModel.Allowance.SecondmentAmount);
            viewModel.Allowance.OthersAmount = cryptoService.Decrypt(viewModel.Allowance.OthersAmount);

            viewModel.Deduction.RoomingChargesAmount = cryptoService.Decrypt(viewModel.Deduction.RoomingChargesAmount);
            viewModel.Deduction.FitnessFirstAmount = cryptoService.Decrypt(viewModel.Deduction.FitnessFirstAmount);
        }

        public void UpdatePayroll(EmployeePayrollViewModel model, int userId)
        {
            var isSalaryChanged = !string.IsNullOrEmpty(model.BasicSalary.OldSalary) && model.BasicSalary.OldSalary != model.BasicSalary.Salary;

            var criteria = new UpdateEmployeePayrollCriteria
            {
                EmployeeId = model.EmployeeId,
                Salary = cryptoService.Encrypt(model.BasicSalary.Salary),
                IsSalaryChanged = isSalaryChanged,
                CurrencyCode = model.BasicSalary.CurrencyCode,
                WorkingDays = model.BasicSalary.WorkingDays,
                EbankingCurrencyCode = model.BasicSalary.EbankingCurrencyCode,
                EbankingAmount = cryptoService.Encrypt(model.BasicSalary.EbankingAmount),
                EbankingAccountId = model.BasicSalary.EbankingAccountId,
                DirectBanking1CurrencyCode = model.BasicSalary.DirectBanking1CurrencyCode,
                DirectBanking1Amount = cryptoService.Encrypt(model.BasicSalary.DirectBanking1Amount),
                DirectBanking1AccountId = model.BasicSalary.DirectBanking1AccountId,
                DirectBanking1BankTypeId = model.BasicSalary.DirectBanking1BankTypeId,
                DirectBanking2CurrencyCode = model.BasicSalary.DirectBanking2CurrencyCode,
                DirectBanking2Amount = cryptoService.Encrypt(model.BasicSalary.DirectBanking2Amount),
                DirectBanking2AccountId = model.BasicSalary.DirectBanking2AccountId,
                DirectBanking2BankTypeId = model.BasicSalary.DirectBanking2BankTypeId,
                CashCurrencyCode = model.BasicSalary.CashCurrencyCode,
                CashAmount = cryptoService.Encrypt(model.BasicSalary.CashAmount),
                CashAccountId = model.BasicSalary.CashAccountId,
                AutoCalculateOn = model.BasicSalary.AutoCalculateOn,
                EffectiveDate = model.BasicSalary.EffectiveDate,
                ReasonId = model.BasicSalary.ReasonId,
                AllowanceCurrencyCode = model.Allowance.CurrencyCode,
                AllowanceDirectBanking1CurrencyCode = model.Allowance.DirectBanking1CurrencyCode,
                AllowanceDirectBanking1Amount = cryptoService.Encrypt(model.Allowance.DirectBanking1Amount),
                AllowanceDirectBanking1AccountId = model.Allowance.DirectBanking1AccountId,
                AllowanceDirectBanking1BankTypeId = model.Allowance.DirectBanking1BankTypeId,
                AllowanceDirectBanking2CurrencyCode = model.Allowance.DirectBanking2CurrencyCode,
                AllowanceDirectBanking2Amount = cryptoService.Encrypt(model.Allowance.DirectBanking2Amount),
                AllowanceDirectBanking2AccountId = model.Allowance.DirectBanking2AccountId,
                AllowanceDirectBanking2BankTypeId = model.Allowance.DirectBanking2BankTypeId,
                AllowanceCashCurrencyCode = model.Allowance.CashCurrencyCode,
                AllowanceCashAmount = cryptoService.Encrypt(model.Allowance.CashAmount),
                AllowanceCashAccountId = model.Allowance.CashAccountId,
                AllowanceAutoCalculateOn = model.Allowance.AutoCalculateOn,
                SubsistenceCurrencyCode = model.Allowance.SubsistenceCurrencyCode,
                SubsistenceAmount = cryptoService.Encrypt(model.Allowance.SubsistenceAmount),
                HomePassageCurrencyCode = model.Allowance.HomePassageCurrencyCode,
                HomePassageAmount = cryptoService.Encrypt(model.Allowance.HomePassageAmount),
                HousingCurrencyCode = model.Allowance.HousingCurrencyCode,
                HousingAmount = cryptoService.Encrypt(model.Allowance.HousingAmount),
                MobilePhoneCurrencyCode = model.Allowance.MobilePhoneCurrencyCode,
                MobilePhoneAmount = cryptoService.Encrypt(model.Allowance.MobilePhoneAmount),
                HPICurrencyCode = model.Allowance.HPICurrencyCode,
                HPIAmount = cryptoService.Encrypt(model.Allowance.HPIAmount),
                DutyMealCurrencyCode = model.Allowance.DutyMealCurrencyCode,
                DutyMealAmount = cryptoService.Encrypt(model.Allowance.DutyMealAmount),
                SecondmentCurrencyCode = model.Allowance.SecondmentCurrencyCode,
                SecondmentAmount = cryptoService.Encrypt(model.Allowance.SecondmentAmount),
                OthersCurrencyCode = model.Allowance.OthersCurrencyCode,
                OthersAmount = cryptoService.Encrypt(model.Allowance.OthersAmount),
                RoomingChargesCurrencyCode = model.Deduction.RoomingChargesCurrencyCode,
                RoomingChargesAmount = cryptoService.Encrypt(model.Deduction.RoomingChargesAmount),
                FitnessFirstCurrencyCode = model.Deduction.FitnessFirstCurrencyCode,
                FitnessFirstAmount = cryptoService.Encrypt(model.Deduction.FitnessFirstAmount),
                UpdatedBy = userId
            };

            baseService.ExecuteNonQuery(criteria);
        }
    }
}