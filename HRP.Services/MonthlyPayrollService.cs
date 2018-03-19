namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
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

        public BackPayViewModel CalculateBackPay(
            DateTime effectiveDate,
            DateTime workingMonth,
            decimal oldBasicSalary,
            decimal newBasicSalary,
            int currencyId)
        {
            var backPay = new BackPayViewModel();
            if (oldBasicSalary != newBasicSalary && workingMonth.Month > effectiveDate.Month)
            {
                int monthDiff = GetMonthDifference(workingMonth, effectiveDate);
                int daysInMonth = DateTime.DaysInMonth(effectiveDate.Year, effectiveDate.Month);
                decimal amount = (((newBasicSalary - oldBasicSalary) / daysInMonth * (daysInMonth - effectiveDate.Day + 1))
                    + ((newBasicSalary - oldBasicSalary) * monthDiff));
                backPay.Amount = RoundAmountWithCurrency(amount, currencyId).ToString(CultureInfo.InvariantCulture);

                backPay.Description = string.Format(Message.BackPayDescriptionFormat, effectiveDate.ToString(Constants.DateFormat));
            }

            return backPay;
        }

        private static int GetMonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
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

        public void CalculateMonthlyBasicAllowance(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance
            , DateTime startDate, DateTime? endDate)
        {
            decimal totalAmount = 0;
            if (!monthlyPayrollBasicAllowance.CurrencyId.HasValue)
            {
                return;
            }

            var currencyId = monthlyPayrollBasicAllowance.CurrencyId.Value;

            totalAmount += ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.SubsistenceAmount,
                monthlyPayrollBasicAllowance.SubsistenceCurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.HomePassageAmount,
                monthlyPayrollBasicAllowance.HomePassageCurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.HousingAmount,
                monthlyPayrollBasicAllowance.HousingCurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.MobilePhoneAmount,
                monthlyPayrollBasicAllowance.MobilePhoneCurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.HPIAmount,
                monthlyPayrollBasicAllowance.HPICurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.DutyMealAmount,
                monthlyPayrollBasicAllowance.DutyMealCurrencyId ?? 0, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.SecondmentAmount,
                monthlyPayrollBasicAllowance.SecondmentCurrencyId ?? 0, currencyId);
            totalAmount += ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.OthersAmount,
                monthlyPayrollBasicAllowance.OthersCurrencyId ?? 0, currencyId);

            totalAmount = RoundAmountWithCurrency(totalAmount, monthlyPayrollBasicAllowance.CurrencyId.Value);
            monthlyPayrollBasicAllowance.TotalAmount = cryptoService.Encrypt(totalAmount.ToString(CultureInfo.InvariantCulture));

            CalculateAfterAdjustment(totalAmount, monthlyPayrollBasicAllowance, startDate, endDate);
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

        private decimal ConvertAmount(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, string amount, int fromCurrencyId, int toCurrencyId)
        {
            decimal.TryParse(cryptoService.Decrypt(amount), out decimal value);
            return value * GetExchangeRate(currencyExchangeRates, fromCurrencyId, toCurrencyId);
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
    }
}