namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.EmployeeManagement;
    using HRP.Models.MonthlyPayroll;
    using HRP.Repositories;
    using HRP.Resources;

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

        public async Task<EmployeeMonthlyPayrollViewModel> GetDataFilterForIndexPageAsync(DateTime month, int companyId, string costCenterIds)
        {
            var criteria = new GetDataFilterForIndexPageCriteria
            {
                UserId = sessionService.UserId,
                Month = month,
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
                await CalculateMonthlyPayroll(viewModel.Month, monthlyPayrolls).ConfigureAwait(false);
            }

            return monthlyPayrolls;
        }

        public async Task<EditMonthlySalaryViewModel> GetMonthlySalaryToEditAsync(int payrollId)
        {
            EditMonthlySalaryViewModel viewModel = null;
            var criteria = new GetMonthlySalaryToEditCriteria
            {
                PayrollId = payrollId,
                UserId = sessionService.UserId
            };

            if (criteria.IsValid())
            {
                viewModel = await monthlyPayrollRepository.GetMonthlySalaryToEditAsync(criteria).ConfigureAwait(false);

                if (viewModel != null)
                {
                    DecryptMonthlySalary(viewModel);
                }
            }

            return viewModel;
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

        public async Task<CommandResult> ResetMonthlySalaryAsync(int payrollId, int companyId)
        {
            var criteria = new ResetMonthlySalaryCriteria
            {
                PayrollId = payrollId,
                CompanyId = companyId,
                UserId = sessionService.UserId
            };
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<CommandResult> EditMonthlySalaryAsync(EditMonthlySalaryViewModel viewModel)
        {
            EncryptMonthlySalaryAddition(viewModel.Additions);
            EncryptMonthlySalaryDeduction(viewModel.Deductions);
            var criteria = new EditMonthlySalaryCriteria
            {
                PayrollId = viewModel.PayrollId,
                CompanyId = viewModel.CompanyId,
                UserId = sessionService.UserId,
                CurrencyId = viewModel.MonthlySalary.CurrencyId,
                Salary = cryptoService.Encrypt(viewModel.MonthlySalary.Salary),
                EBankingCurrencyId = viewModel.MonthlySalary.EBankingCurrencyId,
                EBankingAmount = cryptoService.Encrypt(viewModel.MonthlySalary.EBankingAmount),
                EBankingAccountId = viewModel.MonthlySalary.EBankingAccountId,

                DirectBanking1CurrencyId = viewModel.MonthlySalary.DirectBanking1CurrencyId,
                DirectBanking1Amount = cryptoService.Encrypt(viewModel.MonthlySalary.DirectBanking1Amount),
                DirectBanking1BankTypeId = viewModel.MonthlySalary.DirectBanking1BankTypeId,
                DirectBanking1AccountId = viewModel.MonthlySalary.DirectBanking1AccountId,

                DirectBanking2CurrencyId = viewModel.MonthlySalary.DirectBanking2CurrencyId,
                DirectBanking2Amount = cryptoService.Encrypt(viewModel.MonthlySalary.DirectBanking2Amount),
                DirectBanking2BankTypeId = viewModel.MonthlySalary.DirectBanking2BankTypeId,
                DirectBanking2AccountId = viewModel.MonthlySalary.DirectBanking2AccountId,

                CashCurrencyId = viewModel.MonthlySalary.CashCurrencyId,
                CashAmount = cryptoService.Encrypt(viewModel.MonthlySalary.CashAmount),
                CashAccountId = viewModel.MonthlySalary.CashAccountId,

                AdditionXml = viewModel.Additions.ToXML().ToString(),
                DeductionXml = viewModel.Deductions.ToXML().ToString()
            };
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<MonthlyAllowancesViewModel> GetMonthlyAllowancesAsync(int payrollId)
        {
            var criteria = new GetMonthlyAllowanceCriteria { PayrollId = payrollId };
            var result = await monthlyPayrollRepository.GetMonthlyAllowancesAsync(criteria).ConfigureAwait(false);
            var model = new MonthlyAllowancesViewModel
            {
                Allowance = result.Item1,
                Additions = result.Item2,
                Deductions = result.Item3
            };

            if (model.Allowance != null)
            {
                DecryptMonthlyAllowance(model.Allowance);
            }

            if (model.Additions != null)
            {
                DecryptMonthlyAllowanceAdditions(model.Additions);
            }

            if (model.Deductions != null)
            {
                DecryptMonthlyAllowanceDeductions(model.Deductions);
            }

            return model;
        }

        public async Task<CommandResult> EditMonthlyAllowancesAsync(MonthlyAllowancesViewModel viewModel, int userId)
        {
            var allowanceAdditionXml = ConvertAllowanceAdditionToXml(viewModel.Additions);
            var allowanceDeductionXml = ConvertAllowanceAdditionToXml(viewModel.Deductions);

            var criteria = new EditMonthlyAllowancesCriteria
            {
                PayrollId = viewModel.Allowance.PayrollId,
                CurrencyId = viewModel.Allowance.CurrencyId,
                TotalAmount = viewModel.Allowance.TotalAmount,
                DirectBanking1CurrencyId = viewModel.Allowance.DirectBanking1CurrencyId,
                DirectBanking1Amount = cryptoService.Encrypt(viewModel.Allowance.DirectBanking1Amount),
                DirectBanking1AccountId = viewModel.Allowance.DirectBanking1AccountId,
                DirectBanking1BankTypeId = viewModel.Allowance.DirectBanking1BankTypeId,
                DirectBanking2CurrencyId = viewModel.Allowance.DirectBanking2CurrencyId,
                DirectBanking2Amount = cryptoService.Encrypt(viewModel.Allowance.DirectBanking2Amount),
                DirectBanking2AccountId = viewModel.Allowance.DirectBanking2AccountId,
                DirectBanking2BankTypeId = viewModel.Allowance.DirectBanking2BankTypeId,
                CashCurrencyId = viewModel.Allowance.CashCurrencyId,
                CashAmount = cryptoService.Encrypt(viewModel.Allowance.CashAmount),
                CashAccountId = viewModel.Allowance.CashAccountId,
                AutoCalculateOn = viewModel.Allowance.AutoCalculateOn,
                AllowanceAddition = allowanceAdditionXml,
                AllowanceDeduction = allowanceDeductionXml,
                UserId = userId
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public BackPayAfterAdjustmentViewModel CalculateBackPayAndAfterAdjustment(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            decimal.TryParse(basicSetting.OldSalary, out decimal oldSalary);
            decimal.TryParse(basicSetting.Salary, out decimal newSalary);
            oldSalary = oldSalary * GetExchangeRate(currencyExchangeRates, basicSetting.OldCurrencyId, basicSetting.CurrencyId);
            int daysInMonth = DateTime.DaysInMonth(workingMonth.Year, workingMonth.Month);
            int startDay = groupStartDate.Month == workingMonth.Month ? groupStartDate.Day : 1;
            int endDay = (groupEndDate.HasValue && groupEndDate.Value.Month == workingMonth.Month) ? groupEndDate.Value.Day : daysInMonth;
            decimal amount;
            var result = new BackPayAfterAdjustmentViewModel();

            switch (basicSetting.SalaryChangeReason)
            {
                case BasicSalaryChangeReason.AnnualIncrement:
                    result = CalculateBackPayAndAfterAdjustmentInCaseAnnualIncrement(currencyExchangeRates, workingMonth, basicSetting, groupStartDate, groupEndDate);
                    break;

                case BasicSalaryChangeReason.Promotion:
                case BasicSalaryChangeReason.SalaryAdjustment:
                case BasicSalaryChangeReason.Demotion:
                case BasicSalaryChangeReason.Penalty:
                    if (basicSetting.EffectiveDate.HasValue)
                    {
                        amount = (oldSalary / daysInMonth * (basicSetting.EffectiveDate.Value.Day - startDay))
                            - (newSalary / daysInMonth * (endDay - basicSetting.EffectiveDate.Value.Day + 1));
                        result.AfterAdjustmentAmount = RoundAmountWithCurrency(amount, basicSetting.CurrencyId).ToString(CultureInfo.InvariantCulture);
                        result.Reason = GetAfterAdjustmentReason(workingMonth, basicSetting, groupStartDate, groupEndDate, amount);
                    }
                    break;

                default:
                    amount = newSalary * (endDay - startDay + 1);
                    result.AfterAdjustmentAmount = RoundAmountWithCurrency(amount, basicSetting.CurrencyId).ToString(CultureInfo.InvariantCulture);
                    result.Reason = GetAfterAdjustmentReason(workingMonth, basicSetting, groupStartDate, groupEndDate, amount);
                    break;
            }

            return result;
        }

        public MonthlyPayrollBasicAllowanceModel InitializeMonthlyBasicAllowance(int payrollId, EmployeePayrollAllowanceModel employeePayrollAllowance, EmployeePayrollDeductionModel deduction)
        {
            var monthlyBasicAllowance = new MonthlyPayrollBasicAllowanceModel
            {
                PayrollId = payrollId,
                CurrencyId = employeePayrollAllowance.CurrencyId,
                DirectBanking1CurrencyId = employeePayrollAllowance.DirectBanking1CurrencyId,
                DirectBanking1Amount = employeePayrollAllowance.DirectBanking1Amount,
                DirectBanking1AccountId = employeePayrollAllowance.DirectBanking1AccountId,
                DirectBanking1BankTypeId = employeePayrollAllowance.DirectBanking1BankTypeId,
                DirectBanking2CurrencyId = employeePayrollAllowance.DirectBanking2CurrencyId,
                DirectBanking2Amount = employeePayrollAllowance.DirectBanking2Amount,
                DirectBanking2AccountId = employeePayrollAllowance.DirectBanking2AccountId,
                DirectBanking2BankTypeId = employeePayrollAllowance.DirectBanking2BankTypeId,
                CashCurrencyId = employeePayrollAllowance.CashCurrencyId,
                CashAmount = employeePayrollAllowance.CashAmount,
                CashAccountId = employeePayrollAllowance.CashAccountId,
                AutoCalculateOn = employeePayrollAllowance.AutoCalculateOn,
                SubsistenceCurrencyId = employeePayrollAllowance.SubsistenceCurrencyId,
                SubsistenceAmount = employeePayrollAllowance.SubsistenceAmount,
                HomePassageCurrencyId = employeePayrollAllowance.HomePassageCurrencyId,
                HomePassageAmount = employeePayrollAllowance.HomePassageAmount,
                HousingCurrencyId = employeePayrollAllowance.HousingCurrencyId,
                HousingAmount = employeePayrollAllowance.HousingAmount,
                MobilePhoneCurrencyId = employeePayrollAllowance.MobilePhoneCurrencyId,
                MobilePhoneAmount = employeePayrollAllowance.MobilePhoneAmount,
                HPICurrencyId = employeePayrollAllowance.HPICurrencyId,
                HPIAmount = employeePayrollAllowance.HPIAmount,
                DutyMealCurrencyId = employeePayrollAllowance.DutyMealCurrencyId,
                DutyMealAmount = employeePayrollAllowance.DutyMealAmount,
                SecondmentCurrencyId = employeePayrollAllowance.SecondmentCurrencyId,
                SecondmentAmount = employeePayrollAllowance.SecondmentAmount,
                OthersCurrencyId = employeePayrollAllowance.OthersCurrencyId,
                OthersAmount = employeePayrollAllowance.OthersAmount,
                RoomingChargesCurrencyId = deduction.RoomingChargesCurrencyId,
                RoomingChargesAmount = deduction.RoomingChargesAmount,
                FitnessFirstCurrencyId = deduction.FitnessFirstCurrencyId,
                FitnessFirstAmount = deduction.FitnessFirstAmount,
            };

            return monthlyBasicAllowance;
        }

        public MonthlyAllowanceModel InitializeMonthlyAllowance(int payrollId, MonthlyPayrollBasicAllowanceModel basicAllowance, IEnumerable<MonthlyAllowanceAdditionModel> allowanceAdditions,
            IEnumerable<MonthlyAllowanceDeductionModel> allowanceDeductions, IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            var monthlyAllowance = new MonthlyAllowanceModel
            {
                PayrollId = payrollId,
                CurrencyId = basicAllowance.CurrencyId,
                DirectBanking1CurrencyId = basicAllowance.DirectBanking1CurrencyId,
                DirectBanking1Amount = basicAllowance.DirectBanking1Amount,
                DirectBanking1AccountId = basicAllowance.DirectBanking1AccountId,
                DirectBanking1BankTypeId = basicAllowance.DirectBanking1BankTypeId,
                DirectBanking2CurrencyId = basicAllowance.DirectBanking2CurrencyId,
                DirectBanking2Amount = basicAllowance.DirectBanking2Amount,
                DirectBanking2AccountId = basicAllowance.DirectBanking2AccountId,
                DirectBanking2BankTypeId = basicAllowance.DirectBanking2BankTypeId,
                CashCurrencyId = basicAllowance.CashCurrencyId,
                CashAmount = basicAllowance.CashAmount,
                CashAccountId = basicAllowance.CashAccountId,
                AutoCalculateOn = basicAllowance.AutoCalculateOn,
            };

            if (basicAllowance.CurrencyId.HasValue)
            {
                decimal.TryParse(cryptoService.Decrypt(basicAllowance.AfterAdjustmentAmount), out decimal totalAmount);

                if(basicAllowance.RoomingChargesCurrencyId.HasValue && !string.IsNullOrEmpty(basicAllowance.RoomingChargesAmount))
                {
                    var roomingCharge = ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(basicAllowance.RoomingChargesAmount), 
                        basicAllowance.RoomingChargesCurrencyId.Value, basicAllowance.CurrencyId.Value);
                    totalAmount = totalAmount - roomingCharge;
                }

                if (basicAllowance.FitnessFirstCurrencyId.HasValue && !string.IsNullOrEmpty(basicAllowance.FitnessFirstAmount))
                {
                    var fitnessFirst = ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(basicAllowance.FitnessFirstAmount),
                        basicAllowance.FitnessFirstCurrencyId.Value, basicAllowance.CurrencyId.Value);
                    totalAmount = totalAmount - fitnessFirst;
                }

                var dutyMealAddition = allowanceAdditions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                decimal.TryParse(dutyMealAddition.NetAllowance, out decimal dutyMealAdditionAmount);
                totalAmount = totalAmount + dutyMealAdditionAmount;

                var dutyMealDeduction = allowanceDeductions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                decimal.TryParse(dutyMealDeduction.NetAllowance, out decimal dutyMealDeductionAmount);
                totalAmount = totalAmount - dutyMealDeductionAmount;

                monthlyAllowance.TotalAmount = cryptoService.Encrypt(totalAmount.ToString(CultureInfo.InvariantCulture));
                CalculateAutoAllowance(currencyExchangeRates, monthlyAllowance);
            }

            return monthlyAllowance;
        }

        public void CalculateMonthlyBasicAllowance(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance,
            DateTime startDate, DateTime? endDate)
        {
            decimal totalAmount = 0;

            if (!monthlyPayrollBasicAllowance.CurrencyId.HasValue)
            {
                return;
            }

            var currencyId = monthlyPayrollBasicAllowance.CurrencyId.Value;

            totalAmount += ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyPayrollBasicAllowance.SubsistenceAmount),
                monthlyPayrollBasicAllowance.SubsistenceCurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyPayrollBasicAllowance.HomePassageAmount),
                monthlyPayrollBasicAllowance.HomePassageCurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyPayrollBasicAllowance.HousingAmount),
                monthlyPayrollBasicAllowance.HousingCurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyPayrollBasicAllowance.MobilePhoneAmount),
                monthlyPayrollBasicAllowance.MobilePhoneCurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyPayrollBasicAllowance.HPIAmount),
                monthlyPayrollBasicAllowance.HPICurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyPayrollBasicAllowance.DutyMealAmount),
                monthlyPayrollBasicAllowance.DutyMealCurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyPayrollBasicAllowance.SecondmentAmount),
                monthlyPayrollBasicAllowance.SecondmentCurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyPayrollBasicAllowance.OthersAmount),
                monthlyPayrollBasicAllowance.OthersCurrencyId ?? 0, currencyId);

            totalAmount = RoundAmountWithCurrency(totalAmount, monthlyPayrollBasicAllowance.CurrencyId.Value);
            monthlyPayrollBasicAllowance.TotalAmount = cryptoService.Encrypt(totalAmount.ToString(CultureInfo.InvariantCulture));

            CalculateAfterAdjustment(totalAmount, monthlyPayrollBasicAllowance, startDate, endDate);
            CalculateAutoBasicAllowance(currencyExchangeRates, monthlyPayrollBasicAllowance);
        }

        public IEnumerable<MonthlyAllowanceAdditionModel> InitializeAllowanceAdditions(int payrollId, int standardWorkingDays, int actualWorkingDays,
            EmployeePayrollAllowanceModel employeePayrollAllowance, IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            var additions = new List<MonthlyAllowanceAdditionModel>();
            var types = Enum.GetValues(typeof(AllowanceType)).Cast<AllowanceType>();

            foreach (var type in types)
            {
                additions.Add(new MonthlyAllowanceAdditionModel
                {
                    PayrollId = payrollId,
                    Type = (int)type,
                    CurrencyId = GetCurrencyBaseOnAllowanceType(type, employeePayrollAllowance)
                });
            }

            if (standardWorkingDays < actualWorkingDays)
            {
                var dutyMeal = additions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                dutyMeal.NoOfDays = actualWorkingDays - standardWorkingDays;

                var unitAmount = decimal.Parse(cryptoService.Decrypt(employeePayrollAllowance.DutyMealAmount)) / standardWorkingDays;
                var totalAmount = unitAmount * dutyMeal.NoOfDays;

                dutyMeal.UnitAmount = cryptoService.Encrypt(unitAmount.ToString(CultureInfo.InvariantCulture));
                dutyMeal.TotalAmount = cryptoService.Encrypt(totalAmount.ToString());

                if (dutyMeal.CurrencyId.HasValue && employeePayrollAllowance.CurrencyId.HasValue)
                {
                    var netAllowance = RoundAmountWithCurrency(totalAmount.Value * GetExchangeRate(currencyExchangeRates, dutyMeal.CurrencyId.Value, employeePayrollAllowance.CurrencyId.Value),
                        employeePayrollAllowance.CurrencyId.Value);
                    dutyMeal.NetAllowance = cryptoService.Encrypt(netAllowance.ToString(CultureInfo.InvariantCulture));
                }
            }

            return additions;
        }

        public IEnumerable<MonthlyAllowanceDeductionModel> InitializeAllowanceDeductions(int payrollId, int standardWorkingDays, int actualWorkingDays,
            EmployeePayrollAllowanceModel employeePayrollAllowance, IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            var deductions = new List<MonthlyAllowanceDeductionModel>();
            var types = Enum.GetValues(typeof(AllowanceType)).Cast<AllowanceType>();

            foreach (var type in types)
            {
                deductions.Add(new MonthlyAllowanceDeductionModel
                {
                    PayrollId = payrollId,
                    Type = (int)type,
                    CurrencyId = GetCurrencyBaseOnAllowanceType(type, employeePayrollAllowance)
                });
            }

            if (standardWorkingDays > actualWorkingDays)
            {
                var dutyMeal = deductions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                dutyMeal.NoOfDays = standardWorkingDays - actualWorkingDays;

                var unitAmount = decimal.Parse(cryptoService.Decrypt(employeePayrollAllowance.DutyMealAmount)) / standardWorkingDays;
                var totalAmount = unitAmount * dutyMeal.NoOfDays;

                dutyMeal.UnitAmount = cryptoService.Encrypt(unitAmount.ToString(CultureInfo.InvariantCulture));
                dutyMeal.TotalAmount = cryptoService.Encrypt(totalAmount.ToString());

                if (dutyMeal.CurrencyId.HasValue && employeePayrollAllowance.CurrencyId.HasValue)
                {
                    var netAllowance = RoundAmountWithCurrency(totalAmount.Value * GetExchangeRate(currencyExchangeRates, dutyMeal.CurrencyId.Value, employeePayrollAllowance.CurrencyId.Value),
                        employeePayrollAllowance.CurrencyId.Value);
                    dutyMeal.NetAllowance = cryptoService.Encrypt(netAllowance.ToString(CultureInfo.InvariantCulture));
                }
            }

            return deductions;
        }

        private void CalculateAfterAdjustment(decimal totalAmount, MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance, DateTime startDate, DateTime? endDate)
        {
            var currentDate = DateTime.Today;
            var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);
            var startDay = startDate.Month == currentDate.Month && startDate.Year == currentDate.Year ? startDate.Day : 1;
            var endDay = endDate.HasValue && endDate.Value.Month == currentDate.Month && endDate.Value.Year == currentDate.Year ? endDate.Value.Day : daysInMonth;
            var afterAdjustmentAmount = RoundAmountWithCurrency(totalAmount / daysInMonth * (endDay - startDay + 1), monthlyPayrollBasicAllowance.CurrencyId.Value);

            monthlyPayrollBasicAllowance.AfterAdjustmentAmount = cryptoService.Encrypt(afterAdjustmentAmount.ToString(CultureInfo.InvariantCulture));

            if (afterAdjustmentAmount - totalAmount > 0)
            {
                monthlyPayrollBasicAllowance.Reason = AdjustmentReason.Added.ToString();
            }

            if (afterAdjustmentAmount - totalAmount < 0)
            {
                monthlyPayrollBasicAllowance.Reason = AdjustmentReason.Deducted.ToString();
            }
        }

        private static decimal ConvertAmount(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, string amount, int fromCurrencyId, int toCurrencyId)
        {
            decimal.TryParse(amount, out decimal value);
            return value * GetExchangeRate(currencyExchangeRates, fromCurrencyId, toCurrencyId);
        }

        private static decimal ConvertAmount(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, decimal amount, int fromCurrencyId, int toCurrencyId)
        {
            return amount * GetExchangeRate(currencyExchangeRates, fromCurrencyId, toCurrencyId);
        }

        private void DecryptMonthlyPayroll(IEnumerable<MonthlyPayrollEmployeeInfoViewModel> monthlyPayrolls)
        {
            foreach (MonthlyPayrollEmployeeInfoViewModel monthlyPayroll in monthlyPayrolls)
            {
                monthlyPayroll.BasicSalary.Salary = cryptoService.Decrypt(monthlyPayroll.BasicSalary.Salary);
                monthlyPayroll.BasicSalary.EBankingAmount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.EBankingAmount);
                monthlyPayroll.BasicSalary.DirectBanking1Amount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.DirectBanking1Amount);
                monthlyPayroll.BasicSalary.DirectBanking2Amount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.DirectBanking2Amount);
                monthlyPayroll.BasicSalary.CashAmount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.CashAmount);

                monthlyPayroll.Allowance.Salary = cryptoService.Decrypt(monthlyPayroll.Allowance.Salary);
                monthlyPayroll.Allowance.DirectBanking1Amount = cryptoService.Decrypt(monthlyPayroll.Allowance.DirectBanking1Amount);
                monthlyPayroll.Allowance.DirectBanking2Amount = cryptoService.Decrypt(monthlyPayroll.Allowance.DirectBanking2Amount);
                monthlyPayroll.Allowance.CashAmount = cryptoService.Decrypt(monthlyPayroll.Allowance.CashAmount);
            }
        }

        private async Task CalculateMonthlyPayroll(DateTime month, IEnumerable<MonthlyPayrollEmployeeInfoViewModel> monthlyPayrolls)
        {
            var criteria = new GetExchangeRateCriteria
            {
                Month = month.Month,
                Year = month.Year
            };
            var currencyExchangeRates = await baseService.GetDataAsync<CurrencyExchangeRateModel>(criteria).ConfigureAwait(false);

            foreach (MonthlyPayrollEmployeeInfoViewModel monthlyPayroll in monthlyPayrolls)
            {
                CalculateMonthlyPayrollBasicSalary(monthlyPayroll.BasicSalary, currencyExchangeRates);
                CalculateMonthlyPayrollAllowance(monthlyPayroll.Allowance, currencyExchangeRates);
            }
        }

        private static void CalculateMonthlyPayrollBasicSalary(
            MonthlyPayrollBasicSalaryViewModel basicSalary,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            decimal exchangeRate;
            decimal.TryParse(basicSalary.Salary, out decimal salary);
            decimal.TryParse(basicSalary.EBankingAmount, out decimal ebanking);
            decimal.TryParse(basicSalary.DirectBanking1Amount, out decimal directBanking1);
            decimal.TryParse(basicSalary.DirectBanking2Amount, out decimal directBanking2);
            decimal.TryParse(basicSalary.CashAmount, out decimal cash);
            ebanking = GetExchangeRate(currencyExchangeRates, basicSalary.EbankingCurrencyId, basicSalary.CurrencyId) * ebanking;
            directBanking1 = GetExchangeRate(currencyExchangeRates, basicSalary.DirectBanking1CurrencyId, basicSalary.CurrencyId) * directBanking1;
            directBanking2 = GetExchangeRate(currencyExchangeRates, basicSalary.DirectBanking2CurrencyId, basicSalary.CurrencyId) * directBanking2;
            cash = GetExchangeRate(currencyExchangeRates, basicSalary.CashCurrencyId, basicSalary.CurrencyId) * cash;

            switch (basicSalary.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.Ebanking:
                    exchangeRate = GetExchangeRate(currencyExchangeRates, basicSalary.CurrencyId, basicSalary.EbankingCurrencyId);
                    basicSalary.EBankingAmount = (exchangeRate * (salary - directBanking1 - directBanking2 - cash)).ToString(CultureInfo.InvariantCulture);
                    break;

                case PayrollAutoCalculateOn.DirectBanking1:
                    exchangeRate = GetExchangeRate(currencyExchangeRates, basicSalary.CurrencyId, basicSalary.DirectBanking1CurrencyId);
                    basicSalary.DirectBanking1Amount = (exchangeRate * (salary - directBanking2 - cash)).ToString(CultureInfo.InvariantCulture);
                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    exchangeRate = GetExchangeRate(currencyExchangeRates, basicSalary.CurrencyId, basicSalary.DirectBanking2CurrencyId);
                    basicSalary.DirectBanking2Amount = (exchangeRate * (salary - directBanking1 - cash)).ToString(CultureInfo.InvariantCulture);
                    break;

                case PayrollAutoCalculateOn.Cash:
                    exchangeRate = GetExchangeRate(currencyExchangeRates, basicSalary.CurrencyId, basicSalary.CashCurrencyId);
                    basicSalary.CashAmount = (exchangeRate * (salary - directBanking1 - directBanking2)).ToString(CultureInfo.InvariantCulture);
                    break;

                default:
                    // nothing to do
                    break;
            }
        }

        private static void CalculateMonthlyPayrollAllowance(
            MonthlyPayrollBasicSalaryViewModel allowance,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            decimal exchangeRate;
            decimal.TryParse(allowance.Salary, out decimal salary);
            decimal.TryParse(allowance.DirectBanking1Amount, out decimal directBanking1);
            decimal.TryParse(allowance.DirectBanking2Amount, out decimal directBanking2);
            decimal.TryParse(allowance.CashAmount, out decimal cash);
            directBanking1 = GetExchangeRate(currencyExchangeRates, allowance.DirectBanking1CurrencyId, allowance.CurrencyId) * directBanking1;
            directBanking2 = GetExchangeRate(currencyExchangeRates, allowance.DirectBanking2CurrencyId, allowance.CurrencyId) * directBanking2;
            cash = GetExchangeRate(currencyExchangeRates, allowance.CashCurrencyId, allowance.CurrencyId) * cash;

            switch (allowance.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.DirectBanking1:
                    exchangeRate = GetExchangeRate(currencyExchangeRates, allowance.CurrencyId, allowance.DirectBanking1CurrencyId);
                    allowance.DirectBanking1Amount = (exchangeRate * (salary - directBanking2 - cash)).ToString(CultureInfo.InvariantCulture);
                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    exchangeRate = GetExchangeRate(currencyExchangeRates, allowance.CurrencyId, allowance.DirectBanking2CurrencyId);
                    allowance.DirectBanking2Amount = (exchangeRate * (salary - directBanking1 - cash)).ToString(CultureInfo.InvariantCulture);
                    break;

                case PayrollAutoCalculateOn.Cash:
                    exchangeRate = GetExchangeRate(currencyExchangeRates, allowance.CurrencyId, allowance.CashCurrencyId);
                    allowance.CashAmount = (exchangeRate * (salary - directBanking1 - directBanking2)).ToString(CultureInfo.InvariantCulture);
                    break;

                default:
                    // nothing to do
                    break;
            }
        }

        private static decimal GetExchangeRate(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            int fromCurrencyId,
            int toCurrencyId)
        {
            CurrencyExchangeRateModel currencyExchangeRate =
                currencyExchangeRates.FirstOrDefault(x => x.FromCurrencyId == fromCurrencyId && x.ToCurrencyId == toCurrencyId);

            return currencyExchangeRate != null ? currencyExchangeRate.ExchangeRate : 0;
        }

        private void DecryptMonthlyAllowance(MonthlyAllowanceModel allowance)
        {
            allowance.TotalAmount = cryptoService.Decrypt(allowance.TotalAmount);
            allowance.DirectBanking1Amount = cryptoService.Decrypt(allowance.DirectBanking1Amount);
            allowance.DirectBanking2Amount = cryptoService.Decrypt(allowance.DirectBanking2Amount);
            allowance.CashAmount = cryptoService.Decrypt(allowance.CashAmount);
        }

        private void DecryptMonthlyAllowanceAdditions(IEnumerable<MonthlyAllowanceAdditionModel> additions)
        {
            foreach (var addition in additions)
            {
                addition.UnitAmount = cryptoService.Decrypt(addition.UnitAmount);
                addition.NetAllowance = cryptoService.Decrypt(addition.NetAllowance);
                addition.TotalAmount = cryptoService.Decrypt(addition.TotalAmount);
            }
        }

        private void DecryptMonthlyAllowanceDeductions(IEnumerable<MonthlyAllowanceDeductionModel> deductions)
        {
            foreach (var deduction in deductions)
            {
                deduction.UnitAmount = cryptoService.Decrypt(deduction.UnitAmount);
                deduction.NetAllowance = cryptoService.Decrypt(deduction.NetAllowance);
                deduction.TotalAmount = cryptoService.Decrypt(deduction.TotalAmount);
            }
        }

        private void EncryptMonthlySalaryAddition(IEnumerable<MonthlySalaryAdditionModel> additions)
        {
            foreach (var addition in additions)
            {
                addition.NetSalary = cryptoService.Encrypt(addition.NetSalary);
            }
        }

        private void EncryptMonthlySalaryDeduction(IEnumerable<MonthlySalaryDeductionModel> deductions)
        {
            foreach (var deduction in deductions)
            {
                deduction.NetSalary = cryptoService.Encrypt(deduction.NetSalary);
            }
        }

        private string ConvertAllowanceAdditionToXml(IEnumerable<MonthlyAllowanceAdditionModel> additions)
        {
            var xml = new XElement("Root");

            foreach (var addition in additions)
            {
                var row = new XElement("Row");
                row.SetAttributeValue(nameof(addition.PayrollId), addition.PayrollId);
                row.SetAttributeValue(nameof(addition.Type), addition.Type);
                row.SetAttributeValue(nameof(addition.NoOfDays), addition.NoOfDays);
                row.SetAttributeValue(nameof(addition.UnitAmount), cryptoService.Encrypt(addition.UnitAmount));
                row.SetAttributeValue(nameof(addition.TotalAmount), cryptoService.Encrypt(addition.TotalAmount));
                row.SetAttributeValue(nameof(addition.NetAllowance), cryptoService.Encrypt(addition.NetAllowance));
                row.SetAttributeValue(nameof(addition.CurrencyId), addition.CurrencyId);

                xml.Add(row);
            }

            return xml.ToString();
        }

        private string ConvertAllowanceAdditionToXml(IEnumerable<MonthlyAllowanceDeductionModel> deductions)
        {
            var xml = new XElement("Root");

            foreach (var deduction in deductions)
            {
                var row = new XElement("Row");
                row.SetAttributeValue(nameof(deduction.PayrollId), deduction.PayrollId);
                row.SetAttributeValue(nameof(deduction.Type), deduction.Type);
                row.SetAttributeValue(nameof(deduction.NoOfDays), deduction.NoOfDays);
                row.SetAttributeValue(nameof(deduction.UnitAmount), cryptoService.Encrypt(deduction.UnitAmount));
                row.SetAttributeValue(nameof(deduction.TotalAmount), cryptoService.Encrypt(deduction.TotalAmount));
                row.SetAttributeValue(nameof(deduction.NetAllowance), cryptoService.Encrypt(deduction.NetAllowance));
                row.SetAttributeValue(nameof(deduction.CurrencyId), deduction.CurrencyId);

                xml.Add(row);
            }

            return xml.ToString();
        }

        private void DecryptMonthlySalary(EditMonthlySalaryViewModel viewModel)
        {
            viewModel.MonthlyBasicSalary.Salary = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.Salary);
            viewModel.MonthlyBasicSalary.EBankingAmount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.EBankingAmount);
            viewModel.MonthlyBasicSalary.DirectBanking1Amount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.DirectBanking1Amount);
            viewModel.MonthlyBasicSalary.DirectBanking2Amount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.DirectBanking2Amount);
            viewModel.MonthlyBasicSalary.CashAmount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.CashAmount);

            viewModel.MonthlySalary.Salary = cryptoService.Decrypt(viewModel.MonthlySalary.Salary);
            viewModel.MonthlySalary.EBankingAmount = cryptoService.Decrypt(viewModel.MonthlySalary.EBankingAmount);
            viewModel.MonthlySalary.DirectBanking1Amount = cryptoService.Decrypt(viewModel.MonthlySalary.DirectBanking1Amount);
            viewModel.MonthlySalary.DirectBanking2Amount = cryptoService.Decrypt(viewModel.MonthlySalary.DirectBanking2Amount);
            viewModel.MonthlySalary.CashAmount = cryptoService.Decrypt(viewModel.MonthlySalary.CashAmount);

            foreach (MonthlySalaryAdditionModel addition in viewModel.Additions)
            {
                addition.UnitAmount = cryptoService.Decrypt(addition.UnitAmount);
                addition.NetSalary = cryptoService.Decrypt(addition.NetSalary);
            }

            foreach (MonthlySalaryDeductionModel deduction in viewModel.Deductions)
            {
                deduction.NetSalary = cryptoService.Decrypt(deduction.NetSalary);
            }
        }

        private static decimal RoundAmountWithCurrency(decimal amount, int currencyId)
        {
            if (currencyId == Constants.VietnamCurrencyId)
            {
                return Math.Round(amount / 1000, MidpointRounding.AwayFromZero) * 1000;
            }

            return Math.Round(amount, 2);
        }

        private static int? GetCurrencyBaseOnAllowanceType(AllowanceType type, EmployeePayrollAllowanceModel employeePayrollAllowance)
        {
            switch (type)
            {
                case AllowanceType.Subsistence: return employeePayrollAllowance.SubsistenceCurrencyId;
                case AllowanceType.HomePassage: return employeePayrollAllowance.HomePassageCurrencyId;
                case AllowanceType.Housing: return employeePayrollAllowance.HousingCurrencyId;
                case AllowanceType.MobilePhone: return employeePayrollAllowance.MobilePhoneCurrencyId;
                case AllowanceType.HPI: return employeePayrollAllowance.HPICurrencyId;
                case AllowanceType.DutyMeal: return employeePayrollAllowance.DutyMealCurrencyId;
                case AllowanceType.Secondment: return employeePayrollAllowance.SecondmentCurrencyId;
                case AllowanceType.Others: return employeePayrollAllowance.OthersCurrencyId;
                default: return null;
            }
        }

        private void CalculateAutoBasicAllowance(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance)
        {
            var totalAmount = decimal.Parse(cryptoService.Decrypt(monthlyPayrollBasicAllowance.TotalAmount));
            var currencyId = monthlyPayrollBasicAllowance.CurrencyId ?? 0;
            var directbanking1CurrencyId = monthlyPayrollBasicAllowance.DirectBanking1CurrencyId ?? 0;
            var directbanking2CurrencyId = monthlyPayrollBasicAllowance.DirectBanking2CurrencyId ?? 0;
            var cashCurrencyId = monthlyPayrollBasicAllowance.CashCurrencyId ?? 0;

            var directBanking1 = ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyPayrollBasicAllowance.DirectBanking1Amount), directbanking1CurrencyId, currencyId);
            var directBanking2 = ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyPayrollBasicAllowance.DirectBanking2Amount), directbanking2CurrencyId, currencyId);
            var cash = ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyPayrollBasicAllowance.CashAmount), cashCurrencyId, currencyId);

            switch (monthlyPayrollBasicAllowance.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.DirectBanking1:
                    directBanking1 = RoundAmountWithCurrency(ConvertAmount(currencyExchangeRates, totalAmount - directBanking2 - cash, currencyId, directbanking1CurrencyId), directbanking1CurrencyId);
                    monthlyPayrollBasicAllowance.DirectBanking1Amount = cryptoService.Encrypt(directBanking1.ToString(CultureInfo.InvariantCulture));
                    break;
                case PayrollAutoCalculateOn.DirectBanking2:
                    directBanking2 = RoundAmountWithCurrency(ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - cash, currencyId, directbanking2CurrencyId), directbanking2CurrencyId);
                    monthlyPayrollBasicAllowance.DirectBanking2Amount = cryptoService.Encrypt(directBanking2.ToString(CultureInfo.InvariantCulture));
                    break;

                case PayrollAutoCalculateOn.Cash:
                    cash = RoundAmountWithCurrency(ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - directBanking2, currencyId, cashCurrencyId), cashCurrencyId);
                    monthlyPayrollBasicAllowance.CashAmount = cryptoService.Encrypt(cash.ToString(CultureInfo.InvariantCulture));
                    break;
                default:
                    return;
            }
        }

        private void CalculateAutoAllowance(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, MonthlyAllowanceModel monthlyAllowance)
        {
            var totalAmount = decimal.Parse(cryptoService.Decrypt(monthlyAllowance.TotalAmount));
            var currencyId = monthlyAllowance.CurrencyId ?? 0;
            var directbanking1CurrencyId = monthlyAllowance.DirectBanking1CurrencyId ?? 0;
            var directbanking2CurrencyId = monthlyAllowance.DirectBanking2CurrencyId ?? 0;
            var cashCurrencyId = monthlyAllowance.CashCurrencyId ?? 0;

            var directBanking1 = ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyAllowance.DirectBanking1Amount), directbanking1CurrencyId, currencyId);
            var directBanking2 = ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyAllowance.DirectBanking2Amount), directbanking2CurrencyId, currencyId);
            var cash = ConvertAmount(currencyExchangeRates, cryptoService.Decrypt(monthlyAllowance.CashAmount), cashCurrencyId, currencyId);

            switch (monthlyAllowance.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.DirectBanking1:                  
                    directBanking1 = RoundAmountWithCurrency(ConvertAmount(currencyExchangeRates, totalAmount - directBanking2 - cash, currencyId, directbanking1CurrencyId), directbanking1CurrencyId);
                    monthlyAllowance.DirectBanking1Amount = cryptoService.Encrypt(directBanking1.ToString(CultureInfo.InvariantCulture));
                    break;
                case PayrollAutoCalculateOn.DirectBanking2:
                    directBanking2 = RoundAmountWithCurrency(ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - cash, currencyId, directbanking2CurrencyId), directbanking2CurrencyId);
                    monthlyAllowance.DirectBanking2Amount = cryptoService.Encrypt(directBanking2.ToString(CultureInfo.InvariantCulture));
                    break;

                case PayrollAutoCalculateOn.Cash:
                    cash = RoundAmountWithCurrency(ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - directBanking2, currencyId, cashCurrencyId), cashCurrencyId);
                    monthlyAllowance.CashAmount = cryptoService.Encrypt(cash.ToString(CultureInfo.InvariantCulture));
                    break;
                default:
                    return;
            }
        }

        #region Back Pay and After Adjustment

        private static int GetDifferenceMonth(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 0;
            if (startDate < endDate)
            {
                monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            }

            return Math.Abs(monthsApart);
        }

        private static BackPayAfterAdjustmentViewModel CalculateBackPayAndAfterAdjustmentInCaseAnnualIncrement(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            decimal.TryParse(basicSetting.OldSalary, out decimal oldSalary);
            oldSalary = oldSalary * GetExchangeRate(currencyExchangeRates, basicSetting.OldCurrencyId, basicSetting.CurrencyId);
            decimal.TryParse(basicSetting.Salary, out decimal newSalary);
            int daysInMonth = DateTime.DaysInMonth(workingMonth.Year, workingMonth.Month);
            int startDay = groupStartDate.Month == workingMonth.Month ? groupStartDate.Day : 1;
            int endDay = (groupEndDate.HasValue && groupEndDate.Value.Month == workingMonth.Month) ? groupEndDate.Value.Day : daysInMonth;
            decimal amount;
            var result = new BackPayAfterAdjustmentViewModel();

            if (basicSetting.EffectiveDate.HasValue && oldSalary != newSalary)
            {
                int monthDiff = GetDifferenceMonth(workingMonth, basicSetting.EffectiveDate.Value);
                if (monthDiff > 0)
                {
                    amount = ((newSalary - oldSalary) / daysInMonth * (daysInMonth - basicSetting.EffectiveDate.Value.Day + 1))
                        + ((newSalary - oldSalary) * monthDiff);

                    result.BackPayAmount = RoundAmountWithCurrency(amount, basicSetting.CurrencyId).ToString(CultureInfo.InvariantCulture);
                    result.Description = string.Format(Message.BackPayDescriptionFormat, basicSetting.EffectiveDate.Value.ToString(Constants.DateFormat));
                }
                else
                {
                    amount = (oldSalary / daysInMonth * (basicSetting.EffectiveDate.Value.Day - startDay))
                        - (newSalary / daysInMonth * (endDay - basicSetting.EffectiveDate.Value.Day + 1));

                    result.AfterAdjustmentAmount = RoundAmountWithCurrency(amount, basicSetting.CurrencyId).ToString(CultureInfo.InvariantCulture);
                    result.Reason = GetAfterAdjustmentReason(workingMonth, basicSetting, groupStartDate, groupEndDate, amount);
                }
            }

            return result;
        }

#pragma warning disable S1541 // Methods and properties should not be too complex
#pragma warning disable S3776 // Cognitive Complexity of methods should not be too high
        private static string GetAfterAdjustmentReason(
#pragma warning restore S3776 // Cognitive Complexity of methods should not be too high
#pragma warning restore S1541 // Methods and properties should not be too complex
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            DateTime groupStartDate,
            DateTime? groupEndDate,
            decimal afterAdjustmentAmount)
        {
            decimal.TryParse(basicSetting.OldSalary, out decimal oldSalary);
            decimal.TryParse(basicSetting.Salary, out decimal salary);
            decimal differenceAmount = RoundAmountWithCurrency(afterAdjustmentAmount - salary, basicSetting.CurrencyId);
            var reason = new StringBuilder();
            if (differenceAmount > 0 || differenceAmount < 0)
            {
                reason.AppendFormat(
                    "{0} {1} ({2})",
                    differenceAmount > 0 ? "Added" : "Deducted",
                    Math.Abs(differenceAmount),
                    basicSetting.CurrencyCode);

                if (GetDifferenceMonth(groupStartDate, workingMonth) == 0)
                {
                    if (groupEndDate.HasValue && GetDifferenceMonth(groupEndDate.Value, workingMonth) == 0)
                    {
                        reason.AppendFormat(
                            ". Employee worked from {0} to {1}. ",
                            groupStartDate.ToString(Constants.DateFormat),
                            groupEndDate.Value.ToString(Constants.DateFormat));
                    }
                    else
                    {
                        reason.AppendFormat(
                            ". Employee started working from {0}. ",
                            groupStartDate.ToString(Constants.DateFormat));
                    }
                }
                else
                {
                    if (groupEndDate.HasValue && GetDifferenceMonth(groupEndDate.Value, workingMonth) == 0)
                    {
                        reason.AppendFormat(
                        ". Employee official leaves from {0}. ",
                        groupEndDate.Value.ToString(Constants.DateFormat));
                    }
                }

                switch (basicSetting.SalaryChangeReason)
                {
                    case BasicSalaryChangeReason.Promotion:
                    case BasicSalaryChangeReason.SalaryAdjustment:
                    case BasicSalaryChangeReason.Demotion:
                    case BasicSalaryChangeReason.Penalty:
                        if (groupEndDate.HasValue && GetDifferenceMonth(groupEndDate.Value, workingMonth) == 0)
                        {
                            reason.AppendFormat(
                            "Basic Salary changed on {0}, from {1}({2}) to {3}({4})",
                            basicSetting.EffectiveDate.HasValue ? basicSetting.EffectiveDate.Value.ToString(Constants.DateFormat) : string.Empty,
                            oldSalary,
                            basicSetting.OldCurrencyCode,
                            salary,
                            basicSetting.CurrencyCode);
                        }
                        else
                        {
                            reason.AppendFormat(
                            " for {0} from {1}.",
                            Utilities.GetDescription(basicSetting.SalaryChangeReason.Value),
                            basicSetting.EffectiveDate.HasValue ? basicSetting.EffectiveDate.Value.ToString(Constants.DateFormat) : string.Empty);
                        }
                        break;

                    default:
                        // nothing to do
                        break;
                }
            }

            return reason.ToString();
        }

        #endregion Back Pay and After Adjustment
    }
}