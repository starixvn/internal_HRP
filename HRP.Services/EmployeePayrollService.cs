namespace HRP.Services
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models.ServiceModels;

    public class EmployeePayrollService(
        IBaseService baseService,
        ICryptoService cryptoService) : IEmployeePayrollService
    {
        public async Task<EmployeePayrollModel> GetEmployeePayroll(int employeeId, int userId)
        {
            var criteria = new GetEmployeePayrollCriteria { EmployeeId = employeeId, UserId = userId };
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multi = await db.QueryMultipleAsync(criteria))
                {
                    var viewModel = multi.ReadFirstOrDefault<EmployeePayrollModel>();

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
                        viewModel.Deduction = multi.ReadFirstOrDefault<EmployeePayrollDeductionModel>() ?? new EmployeePayrollDeductionModel { EmployeeId = employeeId };

                        DecryptData(viewModel);
                    }

                    return viewModel;
                }
            }
        }

        public async Task<CommandResult> UpdatePayroll(EmployeePayrollModel model, int userId)
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
                DirectBanking2CurrencyId = model.BasicSalary.DirectBanking2CurrencyId,
                DirectBanking2Amount = cryptoService.Encrypt(model.BasicSalary.DirectBanking2Amount),
                DirectBanking2AccountId = model.BasicSalary.DirectBanking2AccountId,
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
                AllowanceDirectBanking2CurrencyId = model.Allowance.DirectBanking2CurrencyId,
                AllowanceDirectBanking2Amount = cryptoService.Encrypt(model.Allowance.DirectBanking2Amount),
                AllowanceDirectBanking2AccountId = model.Allowance.DirectBanking2AccountId,
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

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        private void DecryptData(EmployeePayrollModel payroll)
        {
            payroll.BasicSalary.Salary = payroll.BasicSalary.OldSalary = cryptoService.Decrypt(payroll.BasicSalary.Salary);
            payroll.BasicSalary.EbankingAmount = cryptoService.Decrypt(payroll.BasicSalary.EbankingAmount);
            payroll.BasicSalary.DirectBanking1Amount = cryptoService.Decrypt(payroll.BasicSalary.DirectBanking1Amount);
            payroll.BasicSalary.DirectBanking2Amount = cryptoService.Decrypt(payroll.BasicSalary.DirectBanking2Amount);
            payroll.BasicSalary.CashAmount = cryptoService.Decrypt(payroll.BasicSalary.CashAmount);

            payroll.Allowance.DirectBanking1Amount = cryptoService.Decrypt(payroll.Allowance.DirectBanking1Amount);
            payroll.Allowance.DirectBanking2Amount = cryptoService.Decrypt(payroll.Allowance.DirectBanking2Amount);
            payroll.Allowance.CashAmount = cryptoService.Decrypt(payroll.Allowance.CashAmount);
            payroll.Allowance.SubsistenceAmount = cryptoService.Decrypt(payroll.Allowance.SubsistenceAmount);
            payroll.Allowance.HomePassageAmount = cryptoService.Decrypt(payroll.Allowance.HomePassageAmount);
            payroll.Allowance.HousingAmount = cryptoService.Decrypt(payroll.Allowance.HousingAmount);
            payroll.Allowance.MobilePhoneAmount = cryptoService.Decrypt(payroll.Allowance.MobilePhoneAmount);
            payroll.Allowance.HPIAmount = cryptoService.Decrypt(payroll.Allowance.HPIAmount);
            payroll.Allowance.DutyMealAmount = cryptoService.Decrypt(payroll.Allowance.DutyMealAmount);
            payroll.Allowance.SecondmentAmount = cryptoService.Decrypt(payroll.Allowance.SecondmentAmount);
            payroll.Allowance.OthersAmount = cryptoService.Decrypt(payroll.Allowance.OthersAmount);

            payroll.Deduction.RoomingChargesAmount = cryptoService.Decrypt(payroll.Deduction.RoomingChargesAmount);
            payroll.Deduction.FitnessFirstAmount = cryptoService.Decrypt(payroll.Deduction.FitnessFirstAmount);
        }
    }
}