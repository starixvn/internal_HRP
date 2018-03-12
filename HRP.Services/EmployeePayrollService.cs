namespace HRP.Services
{
    using System.Threading.Tasks;
    using Criteria.RefResource;
    using Fanex.Data;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.EmployeeManagement;
    using Models.EmployeeManagement.Payroll;

    public class EmployeePayrollService : IEmployeePayrollService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;

        public EmployeePayrollService(IBaseService baseService, ICryptoService cryptoService)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
        }

        public async Task<EmployeePayrollViewModel> GetEmployeePayroll(int employeeId, int userId)
        {
            var criteria = new GetEmployeePayrollCriteria { EmployeeId = employeeId, UserId = userId };
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var viewModel = multi.ReadFirstOrDefault<EmployeePayrollViewModel>();

                    if (viewModel != null)
                    {
                        var basicSalary = new EmployeePayrollBasicSalaryModel
                        {
                            EmployeeId = employeeId,
                            WorkingDays = 22,
                            AutoCalculateOn = PayrollAutoCalculateOn.Cash,
                            InitFirstTime = true
                        };
                        var allowance = new EmployeePayrollAllowanceModel { EmployeeId = employeeId, AutoCalculateOn = PayrollAutoCalculateOn.Cash };

                        viewModel.BasicSalary = multi.ReadFirstOrDefault<EmployeePayrollBasicSalaryModel>() ?? basicSalary;
                        viewModel.BasicSalary.OldCurrencyId = viewModel.BasicSalary.CurrencyId;
                        viewModel.Allowance = multi.ReadFirstOrDefault<EmployeePayrollAllowanceModel>() ?? allowance;
                        viewModel.Deduction = multi.ReadFirstOrDefault<EmployeePayrollDeduction>() ?? new EmployeePayrollDeduction { EmployeeId = employeeId };
                        viewModel.CurrencyExchangeRates = baseService.GetData<CurrencyExchangeRateModel>(new GetLatestExchangeRatesCriteria());
                        viewModel.Currencies = baseService.GetData<CurrencyCodeModel>(new GetActiveCurrencyCodesCriteria());
                        viewModel.ChangeReasons = baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria
                        {
                            RefNames = ReferenceResource.BasicSalaryChangeReason,
                            IsActive = true
                        });
                        viewModel.CashAccounts = baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.PaymentMethodCash, IsActive = true });
                        viewModel.BankTypes = baseService.GetData<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = ReferenceResource.BankType, IsActive = true });

                        DecryptData(viewModel);
                    }

                    return viewModel;
                }
            }
        }

        public async Task<CommandResult> UpdatePayroll(EmployeePayrollViewModel model, int userId)
        {
            var isSalaryChanged = model.BasicSalary.OldSalary != model.BasicSalary.Salary || model.BasicSalary.OldCurrencyId != model.BasicSalary.CurrencyId;

            var criteria = new UpdateEmployeePayrollCriteria
            {
                EmployeeId = model.EmployeeId,
                Salary = cryptoService.Encrypt(model.BasicSalary.Salary),
                IsSalaryChanged = isSalaryChanged,
                CurrencyId = model.BasicSalary.CurrencyId,
                WorkingDays = model.BasicSalary.WorkingDays,
                EbankingCurrencyId = model.BasicSalary.EbankingCurrencyId,
                EbankingAmount = cryptoService.Encrypt(model.BasicSalary.EbankingAmount),
                EbankingAccountId = model.BasicSalary.EbankingAccountId,
                DirectBanking1CurrencyId = model.BasicSalary.DirectBanking1CurrencyId,
                DirectBanking1Amount = cryptoService.Encrypt(model.BasicSalary.DirectBanking1Amount),
                DirectBanking1AccountId = model.BasicSalary.DirectBanking1AccountId,
                DirectBanking1BankTypeId = model.BasicSalary.DirectBanking1BankTypeId,
                DirectBanking2CurrencyId = model.BasicSalary.DirectBanking2CurrencyId,
                DirectBanking2Amount = cryptoService.Encrypt(model.BasicSalary.DirectBanking2Amount),
                DirectBanking2AccountId = model.BasicSalary.DirectBanking2AccountId,
                DirectBanking2BankTypeId = model.BasicSalary.DirectBanking2BankTypeId,
                CashCurrencyId = model.BasicSalary.CashCurrencyId,
                CashAmount = cryptoService.Encrypt(model.BasicSalary.CashAmount),
                CashAccountId = model.BasicSalary.CashAccountId,
                AutoCalculateOn = model.BasicSalary.AutoCalculateOn,
                EffectiveDate = model.BasicSalary.EffectiveDate,
                ReasonId = model.BasicSalary.ReasonId,
                AllowanceCurrencyId = model.Allowance.CurrencyId,
                AllowanceDirectBanking1CurrencyId = model.Allowance.DirectBanking1CurrencyId,
                AllowanceDirectBanking1Amount = cryptoService.Encrypt(model.Allowance.DirectBanking1Amount),
                AllowanceDirectBanking1AccountId = model.Allowance.DirectBanking1AccountId,
                AllowanceDirectBanking1BankTypeId = model.Allowance.DirectBanking1BankTypeId,
                AllowanceDirectBanking2CurrencyId = model.Allowance.DirectBanking2CurrencyId,
                AllowanceDirectBanking2Amount = cryptoService.Encrypt(model.Allowance.DirectBanking2Amount),
                AllowanceDirectBanking2AccountId = model.Allowance.DirectBanking2AccountId,
                AllowanceDirectBanking2BankTypeId = model.Allowance.DirectBanking2BankTypeId,
                AllowanceCashCurrencyId = model.Allowance.CashCurrencyId,
                AllowanceCashAmount = cryptoService.Encrypt(model.Allowance.CashAmount),
                AllowanceCashAccountId = model.Allowance.CashAccountId,
                AllowanceAutoCalculateOn = model.Allowance.AutoCalculateOn,
                SubsistenceCurrencyId = model.Allowance.SubsistenceCurrencyId,
                SubsistenceAmount = cryptoService.Encrypt(model.Allowance.SubsistenceAmount),
                HomePassageCurrencyId = model.Allowance.HomePassageCurrencyId,
                HomePassageAmount = cryptoService.Encrypt(model.Allowance.HomePassageAmount),
                HousingCurrencyId = model.Allowance.HousingCurrencyId,
                HousingAmount = cryptoService.Encrypt(model.Allowance.HousingAmount),
                MobilePhoneCurrencyId = model.Allowance.MobilePhoneCurrencyId,
                MobilePhoneAmount = cryptoService.Encrypt(model.Allowance.MobilePhoneAmount),
                HPICurrencyId = model.Allowance.HPICurrencyId,
                HPIAmount = cryptoService.Encrypt(model.Allowance.HPIAmount),
                DutyMealCurrencyId = model.Allowance.DutyMealCurrencyId,
                DutyMealAmount = cryptoService.Encrypt(model.Allowance.DutyMealAmount),
                SecondmentCurrencyId = model.Allowance.SecondmentCurrencyId,
                SecondmentAmount = cryptoService.Encrypt(model.Allowance.SecondmentAmount),
                OthersCurrencyId = model.Allowance.OthersCurrencyId,
                OthersAmount = cryptoService.Encrypt(model.Allowance.OthersAmount),
                RoomingChargesCurrencyId = model.Deduction.RoomingChargesCurrencyId,
                RoomingChargesAmount = cryptoService.Encrypt(model.Deduction.RoomingChargesAmount),
                FitnessFirstCurrencyId = model.Deduction.FitnessFirstCurrencyId,
                FitnessFirstAmount = cryptoService.Encrypt(model.Deduction.FitnessFirstAmount),
                UpdatedBy = userId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
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
    }
}