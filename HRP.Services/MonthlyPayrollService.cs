namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Extensions;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.MonthlyPayroll;
    using HRP.Models;
    using HRP.Models.Currency;
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

        public async Task<IEnumerable<MonthlyPayrollEmployeeInfoViewModel>> GetMonthlyPayrollAsync(EmployeeMonthlyPayrollViewModel viewModel)
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

            Tuple<IEnumerable<MonthlyPayrollEmployeeInfoViewModel>,
                IEnumerable<MonthlyPayrollBasicSalaryViewModel>,
                IEnumerable<MonthlyPayrollBasicSalaryViewModel>> tuple = await baseService.FetchMultipleAsync<
                    MonthlyPayrollEmployeeInfoViewModel,
                    MonthlyPayrollBasicSalaryViewModel,
                    MonthlyPayrollBasicSalaryViewModel>(criteria).ConfigureAwait(false);

            IEnumerable<MonthlyPayrollEmployeeInfoViewModel> monthlyPayrolls = tuple.Item1;
            foreach (MonthlyPayrollEmployeeInfoViewModel monhtlyPayroll in monthlyPayrolls)
            {
                monhtlyPayroll.BasicSalary = tuple.Item2.FirstOrDefault(x => x.PayrollId == monhtlyPayroll.PayrollId) ?? new MonthlyPayrollBasicSalaryViewModel();
                monhtlyPayroll.Allowance = tuple.Item3.FirstOrDefault(x => x.PayrollId == monhtlyPayroll.PayrollId) ?? new MonthlyPayrollBasicSalaryViewModel();
            }

            DecryptMonthlyPayroll(monthlyPayrolls);

            return monthlyPayrolls;
        }

        public async Task<EditMonthlySalaryViewModel> GetMonthlySalaryAsync(int payrollId)
        {
            EditMonthlySalaryViewModel viewModel = null;
            var criteria = new GetMonthlySalaryToEditCriteria
            {
                PayrollId = payrollId,
                UserId = sessionService.UserId
            };

            Tuple<MonthlyBasicSalaryModel,
                MonthlySalaryModel,
                IEnumerable<MonthlySalaryAdditionModel>,
                IEnumerable<MonthlySalaryDeductionModel>> monthlySalary = await monthlyPayrollRepository.GetMonthlySalaryAsync(criteria).ConfigureAwait(false);

            if (monthlySalary.Item1 != null)
            {
                viewModel = new EditMonthlySalaryViewModel
                {
                    MonthlyBasicSalary = monthlySalary.Item1,
                    MonthlySalary = monthlySalary.Item2 ?? new MonthlySalaryModel(),
                    Additions = monthlySalary.Item3.ToList(),
                    Deductions = monthlySalary.Item4.ToList()
                };
                DecryptAndRoudingAmountMonthlySalary(viewModel);
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

        public async Task<CommandResult> EditMonthlySalaryAsync(EditMonthlySalaryViewModel viewModel)
        {
            EncryptMonthlySalaryAddition(viewModel.Additions);
            EncryptMonthlySalaryDeduction(viewModel.Deductions);
            var criteria = new EditMonthlySalaryCriteria
            {
                PayrollId = viewModel.FilterInfo.PayrollId,
                CompanyId = viewModel.FilterInfo.CompanyId,
                UserId = sessionService.UserId,
                Month = viewModel.Month,
                Salary = cryptoService.Encrypt(viewModel.MonthlySalary.Salary),
                EbankingCurrencyId = viewModel.MonthlySalary.EbankingCurrencyId,
                EbankingAmount = cryptoService.Encrypt(viewModel.MonthlySalary.EbankingAmount),
                EbankingAccountId = viewModel.MonthlySalary.EbankingAccountId,
                DirectBanking1CurrencyId = viewModel.MonthlySalary.DirectBanking1CurrencyId,
                DirectBanking1Amount = cryptoService.Encrypt(viewModel.MonthlySalary.DirectBanking1Amount),
                DirectBanking1AccountId = viewModel.MonthlySalary.DirectBanking1AccountId,
                DirectBanking2CurrencyId = viewModel.MonthlySalary.DirectBanking2CurrencyId,
                DirectBanking2Amount = cryptoService.Encrypt(viewModel.MonthlySalary.DirectBanking2Amount),
                DirectBanking2AccountId = viewModel.MonthlySalary.DirectBanking2AccountId,
                CashCurrencyId = viewModel.MonthlySalary.CashCurrencyId,
                CashAmount = cryptoService.Encrypt(viewModel.MonthlySalary.CashAmount),
                CashAccountId = viewModel.MonthlySalary.CashAccountId,
                AdditionAmount = cryptoService.Encrypt(viewModel.MonthlySalary.AdditionAmount),
                DeductionAmount = cryptoService.Encrypt(viewModel.MonthlySalary.DeductionAmount),
                AutoCalculateOn = viewModel.MonthlySalary.AutoCalculateOn.ToString(),
                AdditionXml = viewModel.Additions.ToXML().ToString(),
                DeductionXml = viewModel.Deductions.ToXML().ToString()
            };
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }

        public async Task<MonthlyAllowancesViewModel> GetMonthlyAllowancesAsync(int payrollId, DateTime month)
        {
            var criteria = new GetMonthlyAllowanceCriteria { PayrollId = payrollId };
            var result = await monthlyPayrollRepository.GetMonthlyAllowancesAsync(criteria).ConfigureAwait(false);
            var model = new MonthlyAllowancesViewModel
            {
                Month = month,
                Allowance = result.Item1 ?? new MonthlyAllowanceModel(),
                Additions = result.Item2.OrderBy(x => x.Type).ToList(),
                Deductions = result.Item3.OrderBy(x => x.Type).ToList(),
                BasicAllowance = result.Item4 ?? new MonthlyPayrollBasicAllowanceModel()
            };

            DecryptMonthlyBasicAllowance(model.BasicAllowance);
            DecryptMonthlyAllowance(model.Allowance);
            DecryptMonthlyAllowanceAdditions(model.Additions);
            DecryptMonthlyAllowanceDeductions(model.Deductions);

            return model;
        }

        public async Task<CommandResult> EditMonthlyAllowancesAsync(MonthlyAllowancesViewModel viewModel, int userId)
        {
            EncryptMonthlyAllowanceAddition(viewModel.Additions);
            EncryptMonthlyAllowanceDeduction(viewModel.Deductions);
            decimal.TryParse(viewModel.Allowance.DirectBanking1Amount, out decimal directbanking1);
            decimal.TryParse(viewModel.Allowance.DirectBanking2Amount, out decimal directbanking2);
            decimal.TryParse(viewModel.Allowance.CashAmount, out decimal cash);
            var status = directbanking1 < 0 || directbanking2 < 0 || cash < 0 ? MonthlyPayrollStatus.Unsatisfied : MonthlyPayrollStatus.Satisfied;

            var criteria = new EditMonthlyAllowancesCriteria
            {
                PayrollId = viewModel.Allowance.PayrollId,
                CurrencyId = viewModel.Allowance.CurrencyId,
                TotalAmount = cryptoService.Encrypt(viewModel.Allowance.TotalAmount),
                DirectBanking1CurrencyId = viewModel.Allowance.DirectBanking1CurrencyId,
                DirectBanking1Amount = cryptoService.Encrypt(viewModel.Allowance.DirectBanking1Amount),
                DirectBanking1AccountId = viewModel.Allowance.DirectBanking1AccountId,
                DirectBanking2CurrencyId = viewModel.Allowance.DirectBanking2CurrencyId,
                DirectBanking2Amount = cryptoService.Encrypt(viewModel.Allowance.DirectBanking2Amount),
                DirectBanking2AccountId = viewModel.Allowance.DirectBanking2AccountId,
                CashCurrencyId = viewModel.Allowance.CashCurrencyId,
                CashAmount = cryptoService.Encrypt(viewModel.Allowance.CashAmount),
                CashAccountId = viewModel.Allowance.CashAccountId,
                AutoCalculateOn = viewModel.Allowance.AutoCalculateOn,
                TotalNetAmount = cryptoService.Encrypt(viewModel.Allowance.TotalNetAmount),
                DSTNetSalaryCurrencyId = viewModel.Allowance.DSTNetSalaryCurrencyId,
                DSTNetSalaryAmount = cryptoService.Encrypt(viewModel.Allowance.DSTNetSalaryAmount),
                AdditionAmount = cryptoService.Encrypt(viewModel.Allowance.AdditionAmount),
                DeductionAmount = cryptoService.Encrypt(viewModel.Allowance.DeductionAmount),
                AllowanceAddition = viewModel.Additions.ToXML().ToString(),
                AllowanceDeduction = viewModel.Deductions.ToXML().ToString(),
                UserId = userId,
                Status = status
            };

            return await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
        }

        public void CalculateBackPayAndAfterAdjustment(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            MonthlySalaryAdditionModel backPay,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            decimal? afterAdjustmentAmount = GetAfterAdjustmentAmount(currencyExchangeRates, workingMonth, basicSetting, groupStartDate, groupEndDate);
            basicSetting.AfterAdjustmentAmount = afterAdjustmentAmount.HasValue ? afterAdjustmentAmount.Value.ToString(CultureInfo.InvariantCulture) : null;
            GetAfterAdjustmentReason(workingMonth, basicSetting, groupStartDate, groupEndDate, afterAdjustmentAmount ?? 0);

            decimal? backPayAmount = GetBackPayAmount(currencyExchangeRates, workingMonth, basicSetting);
            backPay.UnitAmount = backPay.NetSalary = backPayAmount.HasValue ? backPayAmount.Value.ToString(CultureInfo.InvariantCulture) : null;
            backPay.Description = backPayAmount.HasValue && basicSetting.EffectiveDate.HasValue
                ? string.Format(Message.BackPayDescriptionFormat, basicSetting.EffectiveDate.Value.ToString(Constants.DateFormat)) : null;
        }

        public void CalculateMonthlySalary(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            MonthlyBasicSalaryModel basicSalary,
            MonthlySalaryAdditionModel backPay,
            MonthlySalaryModel monthlySalary)
        {
            decimal amount = 0;
            decimal.TryParse(basicSalary.Salary, out decimal salaryAmount);
            decimal.TryParse(basicSalary.AfterAdjustmentAmount, out decimal afterAdjustmentAmount);
            decimal.TryParse(backPay.NetSalary, out decimal backPayAmount);
            decimal ebanking = CalculationService.ConvertAmount(currencyExchangeRates, basicSalary.EbankingAmount, basicSalary.EbankingCurrencyId, basicSalary.CurrencyId);
            decimal directBanking1 = CalculationService.ConvertAmount(currencyExchangeRates, basicSalary.DirectBanking1Amount, basicSalary.DirectBanking1CurrencyId, basicSalary.CurrencyId);
            decimal directBanking2 = CalculationService.ConvertAmount(currencyExchangeRates, basicSalary.DirectBanking2Amount, basicSalary.DirectBanking2CurrencyId, basicSalary.CurrencyId);
            decimal cash = CalculationService.ConvertAmount(currencyExchangeRates, basicSalary.CashAmount, basicSalary.CashCurrencyId, basicSalary.CurrencyId);
            decimal salary = CalculationService.RoundAmountWithCurrency(afterAdjustmentAmount + backPayAmount, basicSalary.CurrencyCode, 0);

            switch (basicSalary.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.Ebanking:
                    monthlySalary.DirectBanking1Amount = basicSalary.DirectBanking1Amount;
                    monthlySalary.DirectBanking2Amount = basicSalary.DirectBanking2Amount;
                    monthlySalary.CashAmount = basicSalary.CashAmount;
                    if (basicSalary.EbankingCurrencyId.HasValue)
                    {
                        amount = CalculationService.ConvertAmount(currencyExchangeRates, salaryAmount - directBanking1 - directBanking2 - cash, basicSalary.CurrencyId, 
                            basicSalary.EbankingCurrencyId.Value);
                        amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.EbankingCurrencyCode, 0);
                        basicSalary.EbankingAmount = amount.ToString(CultureInfo.InvariantCulture);

                        amount = CalculationService.ConvertAmount(currencyExchangeRates, salary - directBanking1 - directBanking2 - cash, basicSalary.CurrencyId, basicSalary.EbankingCurrencyId.Value);
                        amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.EbankingCurrencyCode, 0);
                        monthlySalary.EbankingAmount = amount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.DirectBanking1:
                    monthlySalary.EbankingAmount = basicSalary.EbankingAmount;
                    monthlySalary.DirectBanking2Amount = basicSalary.DirectBanking2Amount;
                    monthlySalary.CashAmount = basicSalary.CashAmount;
                    if (basicSalary.DirectBanking1CurrencyId.HasValue)
                    {
                        amount = CalculationService.ConvertAmount(currencyExchangeRates, salaryAmount - ebanking - directBanking2 - cash, basicSalary.CurrencyId, 
                            basicSalary.DirectBanking1CurrencyId.Value);
                        amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.DirectBanking1CurrencyCode, 0);
                        basicSalary.DirectBanking1Amount = amount.ToString(CultureInfo.InvariantCulture);

                        amount = CalculationService.ConvertAmount(currencyExchangeRates, salary - ebanking - directBanking2 - cash, basicSalary.CurrencyId, basicSalary.DirectBanking1CurrencyId.Value);
                        amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.DirectBanking1CurrencyCode, 0);
                        monthlySalary.DirectBanking1Amount = amount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    monthlySalary.EbankingAmount = basicSalary.EbankingAmount;
                    monthlySalary.DirectBanking1Amount = basicSalary.DirectBanking1Amount;
                    monthlySalary.CashAmount = basicSalary.CashAmount;
                    if (basicSalary.DirectBanking2CurrencyId.HasValue)
                    {
                        amount = CalculationService.ConvertAmount(currencyExchangeRates, salaryAmount - ebanking - directBanking1 - cash, basicSalary.CurrencyId, 
                            basicSalary.DirectBanking2CurrencyId.Value);
                        amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.DirectBanking2CurrencyCode, 0);
                        basicSalary.DirectBanking2Amount = amount.ToString(CultureInfo.InvariantCulture);

                        amount = CalculationService.ConvertAmount(currencyExchangeRates, salary - ebanking - directBanking1 - cash, basicSalary.CurrencyId, basicSalary.DirectBanking2CurrencyId.Value);
                        amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.DirectBanking2CurrencyCode, 0);
                        monthlySalary.DirectBanking2Amount = amount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                default:
                    monthlySalary.EbankingAmount = basicSalary.EbankingAmount;
                    monthlySalary.DirectBanking1Amount = basicSalary.DirectBanking1Amount;
                    monthlySalary.DirectBanking2Amount = basicSalary.DirectBanking2Amount;
                    if (basicSalary.CashCurrencyId.HasValue)
                    {
                        amount = CalculationService.ConvertAmount(currencyExchangeRates, salaryAmount - ebanking - directBanking1 - directBanking2, basicSalary.CurrencyId, 
                            basicSalary.CashCurrencyId.Value);
                        amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.CashCurrencyCode, 0);
                        basicSalary.CashAmount = amount.ToString(CultureInfo.InvariantCulture);

                        amount = CalculationService.ConvertAmount(currencyExchangeRates, salary - ebanking - directBanking1 - directBanking2, basicSalary.CurrencyId, basicSalary.CashCurrencyId.Value);
                        amount = CalculationService.RoundAmountWithCurrency(amount, basicSalary.CashCurrencyCode, 0);
                        monthlySalary.CashAmount = amount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;
            }

            monthlySalary.CurrencyId = basicSalary.CurrencyId;
            monthlySalary.Salary = GetMonthlySalaryAmount(salary, basicSalary.AfterAdjustmentAmount, backPay.NetSalary);
            monthlySalary.EbankingCurrencyId = basicSalary.EbankingCurrencyId;
            monthlySalary.EbankingAccountId = basicSalary.EbankingAccountId;
            monthlySalary.DirectBanking1CurrencyId = basicSalary.DirectBanking1CurrencyId;
            monthlySalary.DirectBanking1AccountId = basicSalary.DirectBanking1AccountId;
            monthlySalary.DirectBanking2CurrencyId = basicSalary.DirectBanking2CurrencyId;
            monthlySalary.DirectBanking2AccountId = basicSalary.DirectBanking2AccountId;
            monthlySalary.CashCurrencyId = basicSalary.CashCurrencyId;
            monthlySalary.CashAccountId = basicSalary.CashAccountId;
            monthlySalary.AutoCalculateOn = basicSalary.AutoCalculateOn;
            monthlySalary.AdditionAmount = backPay.NetSalary;
            monthlySalary.Status = amount < 0 ? MonthlyPayrollStatus.Unsatisfied : MonthlyPayrollStatus.Default;
        }

        public MonthlyAllowanceModel InitializeMonthlyAllowance(
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            IEnumerable<MonthlyAllowanceAdditionModel> allowanceAdditions,
            IEnumerable<MonthlyAllowanceDeductionModel> allowanceDeductions,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            MonthlyDSTNetSalary dstNetSalary)
        {
            var monthlyAllowance = new MonthlyAllowanceModel
            {
                PayrollId = basicAllowance.PayrollId,
                EmployeeId = basicAllowance.EmployeeId,
                CurrencyId = basicAllowance.CurrencyId,
                CurrencyCode = basicAllowance.CurrencyCode,
                DirectBanking1CurrencyId = basicAllowance.DirectBanking1CurrencyId,
                DirectBanking1CurrencyCode = basicAllowance.DirectBanking1CurrencyCode,
                DirectBanking1Amount = basicAllowance.DirectBanking1Amount,
                DirectBanking1AccountId = basicAllowance.DirectBanking1AccountId,
                DirectBanking2CurrencyId = basicAllowance.DirectBanking2CurrencyId,
                DirectBanking2CurrencyCode = basicAllowance.DirectBanking2CurrencyCode,
                DirectBanking2Amount = basicAllowance.DirectBanking2Amount,
                DirectBanking2AccountId = basicAllowance.DirectBanking2AccountId,
                CashCurrencyId = basicAllowance.CashCurrencyId,
                CashCurrencyCode = basicAllowance.CashCurrencyCode,
                CashAmount = basicAllowance.CashAmount,
                CashAccountId = basicAllowance.CashAccountId,
                AutoCalculateOn = basicAllowance.AutoCalculateOn,
                DSTNetSalaryAmount = dstNetSalary?.NetSalaryReceived,
                DSTNetSalaryCurrencyId = dstNetSalary?.CurrencyId
            };

            if (basicAllowance.CurrencyId.HasValue)
            {
                decimal.TryParse(basicAllowance.AfterAdjustmentAmount, out decimal totalAmount);

                if (basicAllowance.RoomingChargesCurrencyId.HasValue && !string.IsNullOrEmpty(basicAllowance.RoomingChargesAmount))
                {
                    var roomingCharge = CalculationService.ConvertAmount(currencyExchangeRates, basicAllowance.RoomingChargesAmount,
                        basicAllowance.RoomingChargesCurrencyId.Value, basicAllowance.CurrencyId.Value);
                    totalAmount = totalAmount - roomingCharge;
                }

                if (basicAllowance.FitnessFirstCurrencyId.HasValue && !string.IsNullOrEmpty(basicAllowance.FitnessFirstAmount))
                {
                    var fitnessFirst = CalculationService.ConvertAmount(currencyExchangeRates, basicAllowance.FitnessFirstAmount,
                        basicAllowance.FitnessFirstCurrencyId.Value, basicAllowance.CurrencyId.Value);
                    totalAmount = totalAmount - fitnessFirst;
                }

                var dutyMealAddition = allowanceAdditions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                decimal.TryParse(dutyMealAddition.NetAllowance, out decimal dutyMealAdditionAmount);
                totalAmount = totalAmount + dutyMealAdditionAmount;
                monthlyAllowance.AdditionAmount = dutyMealAdditionAmount.ToString(CultureInfo.InvariantCulture);

                var dutyMealDeduction = allowanceDeductions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                decimal.TryParse(dutyMealDeduction.NetAllowance, out decimal dutyMealDeductionAmount);
                totalAmount = totalAmount - dutyMealDeductionAmount;
                monthlyAllowance.DeductionAmount = dutyMealDeductionAmount.ToString(CultureInfo.InvariantCulture);

                monthlyAllowance.TotalAmount = CalculationService.RoundAmountWithCurrency(totalAmount, monthlyAllowance.CurrencyCode, 0).ToString(CultureInfo.InvariantCulture);
                CalculateAutoAllowance(currencyExchangeRates, monthlyAllowance);
                CalculateTotalNetAmount(currencyExchangeRates, monthlyAllowance);
            }

            return monthlyAllowance;
        }

        public void CalculateMonthlyBasicAllowance(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance,
            DateTime workingMonth,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            decimal totalAmount = 0;
            if (monthlyPayrollBasicAllowance.CurrencyId.HasValue)
            {
                var currencyId = monthlyPayrollBasicAllowance.CurrencyId.Value;

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.SubsistenceAmount,
                    monthlyPayrollBasicAllowance.SubsistenceCurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.HomePassageAmount,
                    monthlyPayrollBasicAllowance.HomePassageCurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.HousingAmount,
                    monthlyPayrollBasicAllowance.HousingCurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.MobilePhoneAmount,
                    monthlyPayrollBasicAllowance.MobilePhoneCurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.HPIAmount,
                    monthlyPayrollBasicAllowance.HPICurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.DutyMealAmount,
                    monthlyPayrollBasicAllowance.DutyMealCurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.SecondmentAmount,
                    monthlyPayrollBasicAllowance.SecondmentCurrencyId, currencyId);

                totalAmount += CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.OthersAmount,
                    monthlyPayrollBasicAllowance.OthersCurrencyId, currencyId);

                totalAmount = CalculationService.RoundAmountWithCurrency(totalAmount, monthlyPayrollBasicAllowance.CurrencyCode, 0);
                monthlyPayrollBasicAllowance.TotalAmount = totalAmount.ToString(CultureInfo.InvariantCulture);

                CalculateAfterAdjustment(totalAmount, monthlyPayrollBasicAllowance, workingMonth, groupStartDate, groupEndDate, currencyExchangeRates);
                CalculateAutoBasicAllowance(currencyExchangeRates, monthlyPayrollBasicAllowance);
            }
        }

        public IEnumerable<MonthlyAllowanceAdditionModel> InitializeAllowanceAdditions(
            int standardWorkingDays,
            decimal actualWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            var additions = Enum.GetValues(typeof(AllowanceType)).Cast<AllowanceType>()
                .Select(x => new MonthlyAllowanceAdditionModel
                {
                    PayrollId = basicAllowance.PayrollId,
                    EmployeeId = basicAllowance.EmployeeId,
                    Type = (int)x,
                    CurrencyId = GetCurrencyBaseOnAllowanceType(x, basicAllowance)
                }).ToList();

            if (standardWorkingDays < actualWorkingDays)
            {
                var dutyMeal = additions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                dutyMeal.NoOfDays = actualWorkingDays - standardWorkingDays;
                decimal.TryParse(basicAllowance.DutyMealAmount, out decimal dutyMealAmount);

                var unitAmount = dutyMealAmount / standardWorkingDays;
                var totalAmount = unitAmount * dutyMeal.NoOfDays;
                dutyMeal.UnitAmount = unitAmount.ToString(CultureInfo.InvariantCulture);
                dutyMeal.TotalAmount = totalAmount.ToString();

                if (dutyMeal.CurrencyId.HasValue && basicAllowance.CurrencyId.HasValue)
                {
                    var netAllowance = totalAmount.Value * GetExchangeRate(currencyExchangeRates, dutyMeal.CurrencyId.Value, basicAllowance.CurrencyId.Value);
                    dutyMeal.NetAllowance = netAllowance.ToString(CultureInfo.InvariantCulture);
                }
            }

            return additions;
        }

        public IEnumerable<MonthlyAllowanceDeductionModel> InitializeAllowanceDeductions(
            int standardWorkingDays,
            decimal actualWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            var deductions = Enum.GetValues(typeof(AllowanceType)).Cast<AllowanceType>()
                .Select(x => new MonthlyAllowanceDeductionModel
                {
                    PayrollId = basicAllowance.PayrollId,
                    EmployeeId = basicAllowance.EmployeeId,
                    Type = (int)x,
                    CurrencyId = GetCurrencyBaseOnAllowanceType(x, basicAllowance)
                }).ToList();

            if (standardWorkingDays > actualWorkingDays)
            {
                var dutyMeal = deductions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                dutyMeal.NoOfDays = standardWorkingDays - actualWorkingDays;
                decimal.TryParse(basicAllowance.DutyMealAmount, out decimal dutyMealAmount);

                var unitAmount = dutyMealAmount / standardWorkingDays;
                var totalAmount = unitAmount * dutyMeal.NoOfDays;
                dutyMeal.UnitAmount = unitAmount.ToString(CultureInfo.InvariantCulture);
                dutyMeal.TotalAmount = totalAmount.ToString();

                if (dutyMeal.CurrencyId.HasValue && basicAllowance.CurrencyId.HasValue)
                {
                    var netAllowance = totalAmount.Value * GetExchangeRate(currencyExchangeRates, dutyMeal.CurrencyId.Value, basicAllowance.CurrencyId.Value);
                    dutyMeal.NetAllowance = netAllowance.ToString(CultureInfo.InvariantCulture);
                }
            }

            return deductions;
        }

        private static void CalculateAfterAdjustment(
            decimal totalAmount,
            MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance,
            DateTime workingMonth,
            DateTime groupStartDate,
            DateTime? groupEndDate,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRate)
        {
            int daysInMonth = DateTime.DaysInMonth(workingMonth.Year, workingMonth.Month);
            int startDay = GetDifferenceMonth(groupStartDate, workingMonth) == 0 ? groupStartDate.Day : 1;
            int endDay = (groupEndDate.HasValue && GetDifferenceMonth(groupEndDate.Value, workingMonth) == 0) ? groupEndDate.Value.Day : daysInMonth;
            var afterAdjustmentAmount = totalAmount / daysInMonth * (endDay - startDay + 1);
            monthlyPayrollBasicAllowance.AfterAdjustmentAmount = afterAdjustmentAmount.ToString(CultureInfo.InvariantCulture);

            monthlyPayrollBasicAllowance.SubsistenceAfterAdjustmentAmount = 
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.SubsistenceCurrencyId, 
                monthlyPayrollBasicAllowance.SubsistenceAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.HousingAfterAdjustmentAmount = 
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.HousingCurrencyId,
                monthlyPayrollBasicAllowance.HousingAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.HomePassageAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.HomePassageCurrencyId,
                monthlyPayrollBasicAllowance.HomePassageAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.MobilePhoneAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.MobilePhoneCurrencyId,
                monthlyPayrollBasicAllowance.MobilePhoneAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.DutyMealAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.DutyMealCurrencyId,
                monthlyPayrollBasicAllowance.DutyMealAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.HPIAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.HPICurrencyId,
                monthlyPayrollBasicAllowance.HPIAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.OthersAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.OthersCurrencyId,
                monthlyPayrollBasicAllowance.OthersAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            monthlyPayrollBasicAllowance.SecondmentAfterAdjustmentAmount =
                CalculateAfterAdjustmentAllowanceDetail(monthlyPayrollBasicAllowance, monthlyPayrollBasicAllowance.SecondmentCurrencyId,
                monthlyPayrollBasicAllowance.SecondmentAmount, daysInMonth, startDay, endDay, currencyExchangeRate);

            var amount = afterAdjustmentAmount - totalAmount;
            if (amount != 0)
            {
                var reason = new StringBuilder();
                string dateCommencementReason = GetPartOfAfterAdjustmentReason(workingMonth, groupStartDate, groupEndDate);
                reason.AppendFormat("{0} {1} ({2}). ", amount > 0 ? "Added" : "Deducted", Math.Abs(amount), monthlyPayrollBasicAllowance.CurrencyCode);
                reason.Append(dateCommencementReason);

                monthlyPayrollBasicAllowance.Reason = reason.ToString();
            }
        }

        private static string CalculateAfterAdjustmentAllowanceDetail(MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance, int? allowanceDetailCurrencyId,
            string allowanceDetailAmount, int daysInMonth, int startDay, int endDay, IEnumerable<CurrencyExchangeRateModel> currencyExchangeRate)
        {
            if (allowanceDetailCurrencyId.HasValue)
            {
                decimal.TryParse(allowanceDetailAmount, out decimal amount);
                var afterAdjustmentAmount = amount / daysInMonth * (endDay - startDay + 1);
                afterAdjustmentAmount = CalculationService.RoundAllowanceDetailAmountWithCurrency(
                    CalculationService.ConvertAmount(currencyExchangeRate, afterAdjustmentAmount, allowanceDetailCurrencyId, monthlyPayrollBasicAllowance.CurrencyId), 
                    monthlyPayrollBasicAllowance.CurrencyCode);
                return afterAdjustmentAmount.ToString(CultureInfo.InvariantCulture);
            }

            return string.Empty;
        }

        private void DecryptMonthlyPayroll(IEnumerable<MonthlyPayrollEmployeeInfoViewModel> monthlyPayrolls)
        {
            foreach (MonthlyPayrollEmployeeInfoViewModel monthlyPayroll in monthlyPayrolls)
            {
                monthlyPayroll.BasicSalary.Salary = cryptoService.Decrypt(monthlyPayroll.BasicSalary.Salary);
                monthlyPayroll.BasicSalary.EbankingAmount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.EbankingAmount);
                monthlyPayroll.BasicSalary.DirectBanking1Amount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.DirectBanking1Amount);
                monthlyPayroll.BasicSalary.DirectBanking2Amount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.DirectBanking2Amount);
                monthlyPayroll.BasicSalary.CashAmount = cryptoService.Decrypt(monthlyPayroll.BasicSalary.CashAmount);

                monthlyPayroll.Allowance.Salary = cryptoService.Decrypt(monthlyPayroll.Allowance.Salary);
                monthlyPayroll.Allowance.DirectBanking1Amount = cryptoService.Decrypt(monthlyPayroll.Allowance.DirectBanking1Amount);
                monthlyPayroll.Allowance.DirectBanking2Amount = cryptoService.Decrypt(monthlyPayroll.Allowance.DirectBanking2Amount);
                monthlyPayroll.Allowance.CashAmount = cryptoService.Decrypt(monthlyPayroll.Allowance.CashAmount);
            }
        }

        private static decimal GetExchangeRate(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            int fromCurrencyId,
            int toCurrencyId)
        {
            CurrencyExchangeRateModel currencyExchangeRate =
                currencyExchangeRates.FirstOrDefault(x => x.FromCurrencyId == fromCurrencyId && x.ToCurrencyId == toCurrencyId);

            return currencyExchangeRate?.ExchangeRate ?? 0;
        }

        private void DecryptMonthlyBasicAllowance(MonthlyPayrollBasicAllowanceModel basicAllowance)
        {
            basicAllowance.TotalAmount = cryptoService.Decrypt(basicAllowance.TotalAmount);
            basicAllowance.DirectBanking1Amount = cryptoService.Decrypt(basicAllowance.DirectBanking1Amount);
            basicAllowance.DirectBanking2Amount = cryptoService.Decrypt(basicAllowance.DirectBanking2Amount);
            basicAllowance.CashAmount = cryptoService.Decrypt(basicAllowance.CashAmount);
            basicAllowance.AfterAdjustmentAmount = cryptoService.Decrypt(basicAllowance.AfterAdjustmentAmount);
            basicAllowance.FitnessFirstAmount = cryptoService.Decrypt(basicAllowance.FitnessFirstAmount);
            basicAllowance.RoomingChargesAmount = cryptoService.Decrypt(basicAllowance.RoomingChargesAmount);

            basicAllowance.SubsistenceAmount = cryptoService.Decrypt(basicAllowance.SubsistenceAmount);
            basicAllowance.HomePassageAmount = cryptoService.Decrypt(basicAllowance.HomePassageAmount);
            basicAllowance.HousingAmount = cryptoService.Decrypt(basicAllowance.HousingAmount);
            basicAllowance.MobilePhoneAmount = cryptoService.Decrypt(basicAllowance.MobilePhoneAmount);
            basicAllowance.HPIAmount = cryptoService.Decrypt(basicAllowance.HPIAmount);
            basicAllowance.DutyMealAmount = cryptoService.Decrypt(basicAllowance.DutyMealAmount);
            basicAllowance.SecondmentAmount = cryptoService.Decrypt(basicAllowance.SecondmentAmount);
            basicAllowance.OthersAmount = cryptoService.Decrypt(basicAllowance.OthersAmount);

            if(basicAllowance.CurrencyId.HasValue && !string.IsNullOrEmpty(basicAllowance.AfterAdjustmentAmount)) {
                basicAllowance.AfterAdjustmentAmountDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(basicAllowance.AfterAdjustmentAmount), basicAllowance.CurrencyCode)
                    .ToString(CultureInfo.InvariantCulture);
            }
        }

        private void DecryptMonthlyAllowance(MonthlyAllowanceModel allowance)
        {
            allowance.TotalAmount = cryptoService.Decrypt(allowance.TotalAmount);
            allowance.DirectBanking1Amount = cryptoService.Decrypt(allowance.DirectBanking1Amount);
            allowance.DirectBanking2Amount = cryptoService.Decrypt(allowance.DirectBanking2Amount);
            allowance.CashAmount = cryptoService.Decrypt(allowance.CashAmount);
            allowance.TotalNetAmount = cryptoService.Decrypt(allowance.TotalNetAmount);
            allowance.DSTNetSalaryAmount = cryptoService.Decrypt(allowance.DSTNetSalaryAmount);
            allowance.AdditionAmount = cryptoService.Decrypt(allowance.AdditionAmount);
            allowance.DeductionAmount = cryptoService.Decrypt(allowance.DeductionAmount);

            if (allowance.CurrencyId.HasValue)
            {
                if (!string.IsNullOrEmpty(allowance.AdditionAmount))
                {
                    allowance.AdditionAmountDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(allowance.AdditionAmount), allowance.CurrencyCode, 0)
                        .ToString(CultureInfo.InvariantCulture);
                }

                if (!string.IsNullOrEmpty(allowance.DeductionAmount))
                {
                    allowance.DeductionAmountDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(allowance.DeductionAmount), allowance.CurrencyCode, 0)
                        .ToString(CultureInfo.InvariantCulture);
                }
            }
        }

        private void DecryptMonthlyAllowanceAdditions(IEnumerable<MonthlyAllowanceAdditionModel> additions)
        {
            foreach (var addition in additions)
            {
                addition.UnitAmount = cryptoService.Decrypt(addition.UnitAmount);
                addition.NetAllowance = cryptoService.Decrypt(addition.NetAllowance);
                addition.TotalAmount = cryptoService.Decrypt(addition.TotalAmount);

                if (addition.CurrencyId.HasValue)
                {
                    if (!string.IsNullOrEmpty(addition.NetAllowance))
                    {
                        addition.NetAllowanceToDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(addition.NetAllowance), addition.CurrencyCode, 0)
                            .ToString(CultureInfo.InvariantCulture);
                    }

                    if (!string.IsNullOrEmpty(addition.TotalAmount))
                    {
                        addition.TotalAmountToDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(addition.TotalAmount), addition.CurrencyCode)
                            .ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
        }

        private void DecryptMonthlyAllowanceDeductions(IEnumerable<MonthlyAllowanceDeductionModel> deductions)
        {
            foreach (var deduction in deductions)
            {
                deduction.UnitAmount = cryptoService.Decrypt(deduction.UnitAmount);
                deduction.NetAllowance = cryptoService.Decrypt(deduction.NetAllowance);
                deduction.TotalAmount = cryptoService.Decrypt(deduction.TotalAmount);

                if (deduction.CurrencyId.HasValue)
                {
                    if (!string.IsNullOrEmpty(deduction.NetAllowance))
                    {
                        deduction.NetAllowanceToDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(deduction.NetAllowance), deduction.CurrencyCode, 0)
                            .ToString(CultureInfo.InvariantCulture);
                    }

                    if (!string.IsNullOrEmpty(deduction.TotalAmount))
                    {
                        deduction.TotalAmountToDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(deduction.TotalAmount), deduction.CurrencyCode)
                            .ToString(CultureInfo.InvariantCulture);
                    }
                }
            }
        }

        private void EncryptMonthlySalaryAddition(IEnumerable<MonthlySalaryAdditionModel> additions)
        {
            foreach (var addition in additions)
            {
                addition.UnitAmount = cryptoService.Encrypt(addition.UnitAmount);
                addition.NetSalary = cryptoService.Encrypt(addition.NetSalary);
            }
        }

        private void EncryptMonthlySalaryDeduction(IEnumerable<MonthlySalaryDeductionModel> deductions)
        {
            foreach (var deduction in deductions)
            {
                deduction.UnitAmount = cryptoService.Encrypt(deduction.UnitAmount);
                deduction.NetSalary = cryptoService.Encrypt(deduction.NetSalary);
            }
        }

        private void EncryptMonthlyAllowanceAddition(IEnumerable<MonthlyAllowanceAdditionModel> additions)
        {
            foreach (MonthlyAllowanceAdditionModel addition in additions)
            {
                addition.UnitAmount = cryptoService.Encrypt(addition.UnitAmount);
                addition.TotalAmount = cryptoService.Encrypt(addition.TotalAmount);
                addition.NetAllowance = cryptoService.Encrypt(addition.NetAllowance);
            }
        }

        private void EncryptMonthlyAllowanceDeduction(IEnumerable<MonthlyAllowanceDeductionModel> deductions)
        {
            foreach (MonthlyAllowanceDeductionModel deduction in deductions)
            {
                deduction.UnitAmount = cryptoService.Encrypt(deduction.UnitAmount);
                deduction.TotalAmount = cryptoService.Encrypt(deduction.TotalAmount);
                deduction.NetAllowance = cryptoService.Encrypt(deduction.NetAllowance);
            }
        }

        private void DecryptAndRoudingAmountMonthlySalary(EditMonthlySalaryViewModel viewModel)
        {
            viewModel.MonthlyBasicSalary.Salary = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.Salary);
            viewModel.MonthlyBasicSalary.EbankingAmount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.EbankingAmount);
            viewModel.MonthlyBasicSalary.DirectBanking1Amount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.DirectBanking1Amount);
            viewModel.MonthlyBasicSalary.DirectBanking2Amount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.DirectBanking2Amount);
            viewModel.MonthlyBasicSalary.CashAmount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.CashAmount);
            viewModel.MonthlyBasicSalary.AfterAdjustmentAmount = cryptoService.Decrypt(viewModel.MonthlyBasicSalary.AfterAdjustmentAmount);

            viewModel.MonthlySalary.Salary = cryptoService.Decrypt(viewModel.MonthlySalary.Salary);
            viewModel.MonthlySalary.EbankingAmount = cryptoService.Decrypt(viewModel.MonthlySalary.EbankingAmount);
            viewModel.MonthlySalary.DirectBanking1Amount = cryptoService.Decrypt(viewModel.MonthlySalary.DirectBanking1Amount);
            viewModel.MonthlySalary.DirectBanking2Amount = cryptoService.Decrypt(viewModel.MonthlySalary.DirectBanking2Amount);
            viewModel.MonthlySalary.CashAmount = cryptoService.Decrypt(viewModel.MonthlySalary.CashAmount);
            viewModel.MonthlySalary.AdditionAmount = cryptoService.Decrypt(viewModel.MonthlySalary.AdditionAmount);
            viewModel.MonthlySalary.DeductionAmount = cryptoService.Decrypt(viewModel.MonthlySalary.DeductionAmount);

            decimal.TryParse(viewModel.MonthlySalary.AdditionAmount, out decimal amount);
            viewModel.MonthlySalary.AdditionAmountDisplay = !string.IsNullOrWhiteSpace(viewModel.MonthlySalary.AdditionAmount)
                ? CalculationService.RoundAmountWithCurrency(amount, viewModel.MonthlySalary.CurrencyCode).ToString(CultureInfo.InvariantCulture) : string.Empty;
            decimal.TryParse(viewModel.MonthlySalary.DeductionAmount, out amount);
            viewModel.MonthlySalary.DeductionAmountDisplay = !string.IsNullOrWhiteSpace(viewModel.MonthlySalary.DeductionAmount)
                ? CalculationService.RoundAmountWithCurrency(amount, viewModel.MonthlySalary.CurrencyCode).ToString(CultureInfo.InvariantCulture) : string.Empty;

            foreach (MonthlySalaryAdditionModel addition in viewModel.Additions)
            {
                addition.UnitAmount = cryptoService.Decrypt(addition.UnitAmount);
                addition.NetSalary = cryptoService.Decrypt(addition.NetSalary);

                decimal.TryParse(addition.NetSalary, out amount);
                addition.NetSalaryDisplay = !string.IsNullOrWhiteSpace(addition.NetSalary)
                    ? CalculationService.RoundAmountWithCurrency(amount, viewModel.MonthlySalary.CurrencyCode, 0).ToString(CultureInfo.InvariantCulture) : string.Empty;
            }

            foreach (MonthlySalaryDeductionModel deduction in viewModel.Deductions)
            {
                deduction.UnitAmount = cryptoService.Decrypt(deduction.UnitAmount);
                deduction.NetSalary = cryptoService.Decrypt(deduction.NetSalary);

                decimal.TryParse(deduction.NetSalary, out amount);
                deduction.NetSalaryDisplay = !string.IsNullOrWhiteSpace(deduction.NetSalary)
                    ? CalculationService.RoundAmountWithCurrency(amount, viewModel.MonthlySalary.CurrencyCode, 0).ToString(CultureInfo.InvariantCulture) : string.Empty;
            }
        }

        private static int? GetCurrencyBaseOnAllowanceType(AllowanceType type, MonthlyPayrollBasicAllowanceModel basicAllowance)
        {
            switch (type)
            {
                case AllowanceType.Subsistence: return basicAllowance.SubsistenceCurrencyId;
                case AllowanceType.HomePassage: return basicAllowance.HomePassageCurrencyId;
                case AllowanceType.Housing: return basicAllowance.HousingCurrencyId;
                case AllowanceType.MobilePhone: return basicAllowance.MobilePhoneCurrencyId;
                case AllowanceType.HPI: return basicAllowance.HPICurrencyId;
                case AllowanceType.DutyMeal: return basicAllowance.DutyMealCurrencyId;
                case AllowanceType.Secondment: return basicAllowance.SecondmentCurrencyId;
                case AllowanceType.Others: return basicAllowance.OthersCurrencyId;
                default: return null;
            }
        }

        private static void CalculateAutoBasicAllowance(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, MonthlyPayrollBasicAllowanceModel monthlyPayrollBasicAllowance)
        {
            decimal.TryParse(monthlyPayrollBasicAllowance.TotalAmount, out decimal totalAmount);

            var directBanking1 = CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.DirectBanking1Amount,
                monthlyPayrollBasicAllowance.DirectBanking1CurrencyId, monthlyPayrollBasicAllowance.CurrencyId);
            var directBanking2 = CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.DirectBanking2Amount,
                monthlyPayrollBasicAllowance.DirectBanking2CurrencyId, monthlyPayrollBasicAllowance.CurrencyId);
            var cash = CalculationService.ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.CashAmount, monthlyPayrollBasicAllowance.CashCurrencyId, 
                monthlyPayrollBasicAllowance.CurrencyId);

            switch (monthlyPayrollBasicAllowance.AutoCalculateOn)
            { 
                case PayrollAutoCalculateOn.DirectBanking1:
                    if (monthlyPayrollBasicAllowance.DirectBanking1CurrencyId.HasValue)
                    {
                        var convertAmount = CalculationService.ConvertAmount(currencyExchangeRates, totalAmount - directBanking2 - cash, 
                            monthlyPayrollBasicAllowance.CurrencyId, monthlyPayrollBasicAllowance.DirectBanking1CurrencyId);
                        directBanking1 = CalculationService.RoundAmountWithCurrency(convertAmount, monthlyPayrollBasicAllowance.DirectBanking1CurrencyCode, 0);

                        monthlyPayrollBasicAllowance.DirectBanking1Amount = directBanking1.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    if (monthlyPayrollBasicAllowance.DirectBanking2CurrencyId.HasValue)
                    {
                        var convertAmount = CalculationService.ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - cash, monthlyPayrollBasicAllowance.CurrencyId, 
                            monthlyPayrollBasicAllowance.DirectBanking2CurrencyId);
                        directBanking2 = CalculationService.RoundAmountWithCurrency(convertAmount, monthlyPayrollBasicAllowance.DirectBanking2CurrencyCode, 0);

                        monthlyPayrollBasicAllowance.DirectBanking2Amount = directBanking2.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.Cash:
                    if (monthlyPayrollBasicAllowance.CashCurrencyId.HasValue)
                    {
                        var convertAmount = CalculationService.ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - directBanking2, 
                            monthlyPayrollBasicAllowance.CurrencyId, monthlyPayrollBasicAllowance.CashCurrencyId);
                        cash = CalculationService.RoundAmountWithCurrency(convertAmount, monthlyPayrollBasicAllowance.CashCurrencyCode, 0);

                        monthlyPayrollBasicAllowance.CashAmount = cash.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                default:
                    return;
            }
        }

        private static void CalculateAutoAllowance(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, MonthlyAllowanceModel monthlyAllowance)
        {
            decimal.TryParse(monthlyAllowance.TotalAmount, out decimal totalAmount);

            var directBanking1 = CalculationService.ConvertAmount(currencyExchangeRates, monthlyAllowance.DirectBanking1Amount,
                monthlyAllowance.DirectBanking1CurrencyId, monthlyAllowance.CurrencyId);
            var directBanking2 = CalculationService.ConvertAmount(currencyExchangeRates, monthlyAllowance.DirectBanking2Amount,
                monthlyAllowance.DirectBanking2CurrencyId, monthlyAllowance.CurrencyId);
            var cash = CalculationService.ConvertAmount(currencyExchangeRates, monthlyAllowance.CashAmount, monthlyAllowance.CashCurrencyId, monthlyAllowance.CurrencyId);
            decimal autoAmount = 0;

            switch (monthlyAllowance.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.DirectBanking1:
                    if (monthlyAllowance.DirectBanking1CurrencyId.HasValue)
                    {
                        autoAmount = CalculationService.RoundAmountWithCurrency(
                            CalculationService.ConvertAmount(currencyExchangeRates, totalAmount - directBanking2 - cash, monthlyAllowance.CurrencyId, monthlyAllowance.DirectBanking1CurrencyId),
                            monthlyAllowance.DirectBanking1CurrencyCode, 0);
                        monthlyAllowance.DirectBanking1Amount = autoAmount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    if (monthlyAllowance.DirectBanking2CurrencyId.HasValue)
                    {
                        autoAmount = CalculationService.RoundAmountWithCurrency(
                            CalculationService.ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - cash, monthlyAllowance.CurrencyId, monthlyAllowance.DirectBanking2CurrencyId),
                            monthlyAllowance.DirectBanking2CurrencyCode, 0);
                        monthlyAllowance.DirectBanking2Amount = autoAmount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.Cash:
                    if (monthlyAllowance.CashCurrencyId.HasValue)
                    {
                        autoAmount = CalculationService.RoundAmountWithCurrency(
                            CalculationService.ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - directBanking2, monthlyAllowance.CurrencyId, monthlyAllowance.CashCurrencyId),
                            monthlyAllowance.CashCurrencyCode, 0);
                        monthlyAllowance.CashAmount = autoAmount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                default:
                    return;
            }

            monthlyAllowance.Status = autoAmount >= 0 ? MonthlyPayrollStatus.Default : MonthlyPayrollStatus.Unsatisfied;
        }

        private static void CalculateTotalNetAmount(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, MonthlyAllowanceModel monthlyAllowance)
        {
            if (monthlyAllowance.DSTNetSalaryCurrencyId.HasValue && !string.IsNullOrEmpty(monthlyAllowance.DSTNetSalaryAmount))
            {
                decimal.TryParse(monthlyAllowance.TotalAmount, out decimal totalAmount);
                var totalNetAmount = totalAmount -
                    CalculationService.ConvertAmount(currencyExchangeRates, monthlyAllowance.DSTNetSalaryAmount, monthlyAllowance.DSTNetSalaryCurrencyId.Value, monthlyAllowance.CurrencyId.Value);

                monthlyAllowance.TotalNetAmount = CalculationService.RoundAmountWithCurrency(totalNetAmount, monthlyAllowance.CurrencyCode).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                monthlyAllowance.TotalNetAmount = monthlyAllowance.TotalAmount;
            }
        }

        #region Back Pay and After Adjustment

        private static int GetDifferenceMonth(DateTime past, DateTime now)
        {
            int monthsApart = 12 * (past.Year - now.Year) + past.Month - now.Month;
            return Math.Abs(monthsApart);
        }

        private static string GetMonthlySalaryAmount(decimal monthlySalaryAmount, string afterAdjustmentAmount, string backPayAmount)
        {
            return !string.IsNullOrWhiteSpace(afterAdjustmentAmount) || !string.IsNullOrWhiteSpace(backPayAmount)
                ? monthlySalaryAmount.ToString(CultureInfo.InvariantCulture) : null;
        }

        private static decimal? GetAfterAdjustmentAmount(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            decimal? amount = null;
            if (!string.IsNullOrWhiteSpace(basicSetting.Salary))
            {
                var currencyId = basicSetting.CurrencyId ?? 0;
                decimal.TryParse(basicSetting.OldSalary, out decimal oldSalary);
                decimal.TryParse(basicSetting.Salary, out decimal newSalary);
                oldSalary = oldSalary * GetExchangeRate(currencyExchangeRates, basicSetting.OldCurrencyId, currencyId);
                int daysInMonth = DateTime.DaysInMonth(workingMonth.Year, workingMonth.Month);
                int startDay = GetDifferenceMonth(groupStartDate, workingMonth) == 0 ? groupStartDate.Day : 1;
                int endDay = (groupEndDate.HasValue && GetDifferenceMonth(groupEndDate.Value, workingMonth) == 0) ? groupEndDate.Value.Day : daysInMonth;

                amount = (basicSetting.EffectiveDate.HasValue
                    && GetDifferenceMonth(basicSetting.EffectiveDate.Value, workingMonth) == 0
                    && oldSalary != newSalary)
                        ? (oldSalary / daysInMonth * (basicSetting.EffectiveDate.Value.Day - startDay))
                            + (newSalary / daysInMonth * (endDay - basicSetting.EffectiveDate.Value.Day + 1))
                        : newSalary / daysInMonth * (endDay - startDay + 1);

                amount = CalculationService.RoundAllowanceDetailAmountWithCurrency(amount.Value, basicSetting.CurrencyCode);
            }

            return amount;
        }

        private static decimal? GetBackPayAmount(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting)
        {
            var currencyId = basicSetting.CurrencyId ?? 0;
            decimal.TryParse(basicSetting.OldSalary, out decimal oldSalary);
            decimal.TryParse(basicSetting.Salary, out decimal newSalary);
            oldSalary = oldSalary * GetExchangeRate(currencyExchangeRates, basicSetting.OldCurrencyId, currencyId);
            decimal? backPayAmount = null;

            if (basicSetting.SalaryChangeReason == BasicSalaryChangeReason.AnnualIncrement
                && basicSetting.EffectiveDate.HasValue
                && oldSalary != newSalary)
            {
                int monthDiff = GetDifferenceMonth(basicSetting.EffectiveDate.Value, workingMonth);
                if (monthDiff > 0)
                {
                    var daysInMonth = DateTime.DaysInMonth(basicSetting.EffectiveDate.Value.Year, basicSetting.EffectiveDate.Value.Month);
                    backPayAmount = ((newSalary - oldSalary) / daysInMonth * (daysInMonth - basicSetting.EffectiveDate.Value.Day + 1))
                        + ((newSalary - oldSalary) * (monthDiff - 1));

                    backPayAmount = CalculationService.RoundAllowanceDetailAmountWithCurrency(backPayAmount.Value, basicSetting.CurrencyCode);
                }
            }

            return backPayAmount;
        }

        private static void GetAfterAdjustmentReason(
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            DateTime groupStartDate,
            DateTime? groupEndDate,
            decimal afterAdjustmentAmount)
        {
            decimal.TryParse(basicSetting.OldSalary, out decimal oldSalary);
            decimal.TryParse(basicSetting.Salary, out decimal salary);
            decimal differenceAmount = CalculationService.RoundAllowanceDetailAmountWithCurrency(afterAdjustmentAmount - salary, basicSetting.CurrencyCode);
            var reason = new StringBuilder();
            if (differenceAmount != 0)
            {
                string dateCommencementReason = GetPartOfAfterAdjustmentReason(workingMonth, groupStartDate, groupEndDate);
                reason.AppendFormat("{0} {1} ({2}).", differenceAmount > 0 ? "Added" : "Deducted", Math.Abs(differenceAmount), basicSetting.CurrencyCode);
                reason.Append(dateCommencementReason);

                switch (basicSetting.SalaryChangeReason)
                {
                    case BasicSalaryChangeReason.AnnualIncrement:
                    case BasicSalaryChangeReason.Promotion:
                    case BasicSalaryChangeReason.SalaryAdjustment:
                    case BasicSalaryChangeReason.Demotion:
                    case BasicSalaryChangeReason.Penalty:
                        reason.AppendFormat(
                            " Basic Salary changed on {0} for {1}.",
                            basicSetting.EffectiveDate.HasValue ? basicSetting.EffectiveDate.Value.ToString(Constants.DateFormat) : string.Empty,
                            EnumHelper.GetDescription(basicSetting.SalaryChangeReason.Value));
                        break;

                    default:
                        // nothing to do
                        break;
                }
            }

            basicSetting.AfterAdjustmentReason = reason.ToString();
        }

        private static string GetPartOfAfterAdjustmentReason(
            DateTime workingMonth,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            int differenceEndDate = groupEndDate.HasValue ? GetDifferenceMonth(groupEndDate.Value, workingMonth) : -1;
            var reason = new StringBuilder();
            if (GetDifferenceMonth(groupStartDate, workingMonth) == 0)
            {
                if (differenceEndDate == 0)
                {
                    reason.AppendFormat(
                        " Employee worked from {0} to {1}.",
                        groupStartDate.ToString(Constants.DateFormat),
                        groupEndDate.Value.ToString(Constants.DateFormat));
                }
                else
                {
                    reason.AppendFormat(" Employee started working from {0}.", groupStartDate.ToString(Constants.DateFormat));
                }
            }
            else
            {
                if (differenceEndDate == 0)
                {
                    reason.AppendFormat(" Employee official leaves from {0}.", groupEndDate.Value.ToString(Constants.DateFormat));
                }
            }

            return reason.ToString();
        }

        #endregion Back Pay and After Adjustment
    }
}