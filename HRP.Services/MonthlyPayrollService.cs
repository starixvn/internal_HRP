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
                Employee = viewModel.Employee,
                IDNo = viewModel.IDNo,
                WorkingDays = viewModel.WorkingDays
            };

            if (criteria.IsValid())
            {
                monthlyPayrolls = await monthlyPayrollRepository.GetMonthlyPayrollAsync(criteria).ConfigureAwait(false);
                DecryptMonthlyPayroll(monthlyPayrolls);
            }

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

            Tuple<MonthlyPayrollFilterInfoModel,
                MonthlyBasicSalaryModel,
                MonthlySalaryModel,
                IEnumerable<MonthlySalaryAdditionModel>,
                IEnumerable<MonthlySalaryDeductionModel>> monthlySalary = await monthlyPayrollRepository.GetMonthlySalaryAsync(criteria).ConfigureAwait(false);

            if (monthlySalary.Item1 != null)
            {
                viewModel = new EditMonthlySalaryViewModel
                {
                    FilterInfo = monthlySalary.Item1,
                    MonthlyBasicSalary = monthlySalary.Item2,
                    MonthlySalary = monthlySalary.Item3,
                    Additions = monthlySalary.Item4.ToList(),
                    Deductions = monthlySalary.Item5.ToList()
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
                BasicAllowance = result.Item4,
                EmployeeInfo = result.Item5
            };

            if (model.BasicAllowance != null)
            {
                DecryptMonthlyBasicAllowance(model.BasicAllowance);
            }

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
                AllowanceAddition = allowanceAdditionXml,
                AllowanceDeduction = allowanceDeductionXml,
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
            switch (basicSetting.SalaryChangeReason)
            {
                case BasicSalaryChangeReason.AnnualIncrement:
                case BasicSalaryChangeReason.Promotion:
                case BasicSalaryChangeReason.SalaryAdjustment:
                case BasicSalaryChangeReason.Demotion:
                case BasicSalaryChangeReason.Penalty:
                    CalculateBackPayAndAfterAdjustmentInCaseSalaryChangeReason(currencyExchangeRates, workingMonth, basicSetting, backPay, groupStartDate, groupEndDate);
                    break;

                default:
                    CalculateBackPayAndAfterAdjustmentInCaseGroupDateCommencement(workingMonth, basicSetting, groupStartDate, groupEndDate);
                    break;
            }
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
            decimal ebanking = ConvertAmount(currencyExchangeRates, basicSalary.EbankingAmount, basicSalary.EbankingCurrencyId, basicSalary.CurrencyId);
            decimal directBanking1 = ConvertAmount(currencyExchangeRates, basicSalary.DirectBanking1Amount, basicSalary.DirectBanking1CurrencyId, basicSalary.CurrencyId);
            decimal directBanking2 = ConvertAmount(currencyExchangeRates, basicSalary.DirectBanking2Amount, basicSalary.DirectBanking2CurrencyId, basicSalary.CurrencyId);
            decimal cash = ConvertAmount(currencyExchangeRates, basicSalary.CashAmount, basicSalary.CashCurrencyId, basicSalary.CurrencyId);
            decimal salary = RoundAmountWithCurrency(afterAdjustmentAmount + backPayAmount, basicSalary.CurrencyId ?? 0, 0);

            switch (basicSalary.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.Ebanking:
                    monthlySalary.DirectBanking1Amount = basicSalary.DirectBanking1Amount;
                    monthlySalary.DirectBanking2Amount = basicSalary.DirectBanking2Amount;
                    monthlySalary.CashAmount = basicSalary.CashAmount;
                    if (basicSalary.EbankingCurrencyId.HasValue)
                    {
                        amount = ConvertAmount(currencyExchangeRates, salaryAmount - directBanking1 - directBanking2 - cash, basicSalary.CurrencyId, basicSalary.EbankingCurrencyId.Value);
                        amount = RoundAmountWithCurrency(amount, basicSalary.EbankingCurrencyId.Value, 0);
                        basicSalary.EbankingAmount = amount.ToString(CultureInfo.InvariantCulture);

                        amount = ConvertAmount(currencyExchangeRates, salary - directBanking1 - directBanking2 - cash, basicSalary.CurrencyId, basicSalary.EbankingCurrencyId.Value);
                        amount = RoundAmountWithCurrency(amount, basicSalary.EbankingCurrencyId.Value, 0);
                        monthlySalary.EbankingAmount = amount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.DirectBanking1:
                    monthlySalary.EbankingAmount = basicSalary.EbankingAmount;
                    monthlySalary.DirectBanking2Amount = basicSalary.DirectBanking2Amount;
                    monthlySalary.CashAmount = basicSalary.CashAmount;
                    if (basicSalary.DirectBanking1CurrencyId.HasValue)
                    {
                        amount = ConvertAmount(currencyExchangeRates, salaryAmount - ebanking - directBanking2 - cash, basicSalary.CurrencyId, basicSalary.DirectBanking1CurrencyId.Value);
                        amount = RoundAmountWithCurrency(amount, basicSalary.DirectBanking1CurrencyId.Value, 0);
                        basicSalary.DirectBanking1Amount = amount.ToString(CultureInfo.InvariantCulture);

                        amount = ConvertAmount(currencyExchangeRates, salary - ebanking - directBanking2 - cash, basicSalary.CurrencyId, basicSalary.DirectBanking1CurrencyId.Value);
                        amount = RoundAmountWithCurrency(amount, basicSalary.DirectBanking1CurrencyId.Value, 0);
                        monthlySalary.DirectBanking1Amount = amount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    monthlySalary.EbankingAmount = basicSalary.EbankingAmount;
                    monthlySalary.DirectBanking1Amount = basicSalary.DirectBanking1Amount;
                    monthlySalary.CashAmount = basicSalary.CashAmount;
                    if (basicSalary.DirectBanking2CurrencyId.HasValue)
                    {
                        amount = ConvertAmount(currencyExchangeRates, salaryAmount - ebanking - directBanking1 - cash, basicSalary.CurrencyId, basicSalary.DirectBanking2CurrencyId.Value);
                        amount = RoundAmountWithCurrency(amount, basicSalary.DirectBanking2CurrencyId.Value, 0);
                        basicSalary.DirectBanking2Amount = amount.ToString(CultureInfo.InvariantCulture);

                        amount = ConvertAmount(currencyExchangeRates, salary - ebanking - directBanking1 - cash, basicSalary.CurrencyId, basicSalary.DirectBanking2CurrencyId.Value);
                        amount = RoundAmountWithCurrency(amount, basicSalary.DirectBanking2CurrencyId.Value, 0);
                        monthlySalary.DirectBanking2Amount = amount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                default:
                    monthlySalary.EbankingAmount = basicSalary.EbankingAmount;
                    monthlySalary.DirectBanking1Amount = basicSalary.DirectBanking1Amount;
                    monthlySalary.DirectBanking2Amount = basicSalary.DirectBanking2Amount;
                    if (basicSalary.CashCurrencyId.HasValue)
                    {
                        amount = ConvertAmount(currencyExchangeRates, salaryAmount - ebanking - directBanking1 - directBanking2, basicSalary.CurrencyId, basicSalary.CashCurrencyId.Value);
                        amount = RoundAmountWithCurrency(amount, basicSalary.CashCurrencyId.Value, 0);
                        basicSalary.CashAmount = amount.ToString(CultureInfo.InvariantCulture);

                        amount = ConvertAmount(currencyExchangeRates, salary - ebanking - directBanking1 - directBanking2, basicSalary.CurrencyId, basicSalary.CashCurrencyId.Value);
                        amount = RoundAmountWithCurrency(amount, basicSalary.CashCurrencyId.Value, 0);
                        monthlySalary.CashAmount = amount.ToString(CultureInfo.InvariantCulture);
                    }

                    break;
            }

            monthlySalary.CurrencyId = basicSalary.CurrencyId;
            monthlySalary.Salary = salary.ToString(CultureInfo.InvariantCulture);
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
                DirectBanking1CurrencyId = basicAllowance.DirectBanking1CurrencyId,
                DirectBanking1Amount = basicAllowance.DirectBanking1Amount,
                DirectBanking1AccountId = basicAllowance.DirectBanking1AccountId,
                DirectBanking2CurrencyId = basicAllowance.DirectBanking2CurrencyId,
                DirectBanking2Amount = basicAllowance.DirectBanking2Amount,
                DirectBanking2AccountId = basicAllowance.DirectBanking2AccountId,
                CashCurrencyId = basicAllowance.CashCurrencyId,
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
                    var roomingCharge = ConvertAmount(currencyExchangeRates, basicAllowance.RoomingChargesAmount,
                        basicAllowance.RoomingChargesCurrencyId.Value, basicAllowance.CurrencyId.Value);
                    totalAmount = totalAmount - roomingCharge;
                }

                if (basicAllowance.FitnessFirstCurrencyId.HasValue && !string.IsNullOrEmpty(basicAllowance.FitnessFirstAmount))
                {
                    var fitnessFirst = ConvertAmount(currencyExchangeRates, basicAllowance.FitnessFirstAmount,
                        basicAllowance.FitnessFirstCurrencyId.Value, basicAllowance.CurrencyId.Value);
                    totalAmount = totalAmount - fitnessFirst;
                }

                var dutyMealAddition = allowanceAdditions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                decimal.TryParse(dutyMealAddition.NetAllowance, out decimal dutyMealAdditionAmount);
                totalAmount = totalAmount + dutyMealAdditionAmount;

                var dutyMealDeduction = allowanceDeductions.FirstOrDefault(x => x.Type == (int)AllowanceType.DutyMeal);
                decimal.TryParse(dutyMealDeduction.NetAllowance, out decimal dutyMealDeductionAmount);
                totalAmount = totalAmount - dutyMealDeductionAmount;

                monthlyAllowance.TotalAmount = RoundAmountWithCurrency(totalAmount, monthlyAllowance.CurrencyId.Value, 0).ToString(CultureInfo.InvariantCulture);
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

            totalAmount = RoundAmountWithCurrency(totalAmount, monthlyPayrollBasicAllowance.CurrencyId.Value, 0);
            monthlyPayrollBasicAllowance.TotalAmount = totalAmount.ToString(CultureInfo.InvariantCulture);

            CalculateAfterAdjustment(totalAmount, monthlyPayrollBasicAllowance, workingMonth, groupStartDate, groupEndDate);
            CalculateAutoBasicAllowance(currencyExchangeRates, monthlyPayrollBasicAllowance);
        }

        public IEnumerable<MonthlyAllowanceAdditionModel> InitializeAllowanceAdditions(
            int standardWorkingDays,
            decimal actualWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            var additions = new List<MonthlyAllowanceAdditionModel>();
            var types = Enum.GetValues(typeof(AllowanceType)).Cast<AllowanceType>();

            foreach (var type in types)
            {
                additions.Add(new MonthlyAllowanceAdditionModel
                {
                    PayrollId = basicAllowance.PayrollId,
                    EmployeeId = basicAllowance.EmployeeId,
                    Type = (int)type,
                    CurrencyId = GetCurrencyBaseOnAllowanceType(type, basicAllowance)
                });
            }

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
            var deductions = new List<MonthlyAllowanceDeductionModel>();
            var types = Enum.GetValues(typeof(AllowanceType)).Cast<AllowanceType>();

            foreach (var type in types)
            {
                deductions.Add(new MonthlyAllowanceDeductionModel
                {
                    PayrollId = basicAllowance.PayrollId,
                    EmployeeId = basicAllowance.EmployeeId,
                    Type = (int)type,
                    CurrencyId = GetCurrencyBaseOnAllowanceType(type, basicAllowance)
                });
            }

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
            DateTime? groupEndDate)
        {
            int daysInMonth = DateTime.DaysInMonth(workingMonth.Year, workingMonth.Month);
            int startDay = GetDifferenceMonth(groupStartDate, workingMonth) == 0 ? groupStartDate.Day : 1;
            int endDay = (groupEndDate.HasValue && GetDifferenceMonth(groupEndDate.Value, workingMonth) == 0) ? groupEndDate.Value.Day : daysInMonth;
            var afterAdjustmentAmount = RoundAmountWithCurrency(totalAmount / daysInMonth * (endDay - startDay + 1), monthlyPayrollBasicAllowance.CurrencyId.Value);

            monthlyPayrollBasicAllowance.AfterAdjustmentAmount = afterAdjustmentAmount.ToString(CultureInfo.InvariantCulture);
            var amount = afterAdjustmentAmount - totalAmount;
            if (amount != 0)
            {
                var reason = new StringBuilder();
                reason.AppendFormat("{0} {1} ({2}). ", amount > 0 ? "Added" : "Deducted", Math.Abs(amount), monthlyPayrollBasicAllowance.CurrencyCode);

                string dateCommencementReason = GetPartOfAfterAdjustmentReason(workingMonth, groupStartDate, groupEndDate);

                reason.Append(dateCommencementReason);

                monthlyPayrollBasicAllowance.Reason = reason.ToString();
            }
        }

        private static decimal ConvertAmount(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            string amount,
            int? fromCurrencyId,
            int? toCurrencyId)
        {
            decimal.TryParse(amount, out decimal value);
            return value * GetExchangeRate(currencyExchangeRates, fromCurrencyId ?? 0, toCurrencyId ?? 0);
        }

        private static decimal ConvertAmount(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            decimal amount,
            int? fromCurrencyId,
            int? toCurrencyId)
        {
            return amount * GetExchangeRate(currencyExchangeRates, fromCurrencyId ?? 0, toCurrencyId ?? 0);
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
        }

        private void DecryptMonthlyAllowance(MonthlyAllowanceModel allowance)
        {
            allowance.TotalAmount = cryptoService.Decrypt(allowance.TotalAmount);
            allowance.DirectBanking1Amount = cryptoService.Decrypt(allowance.DirectBanking1Amount);
            allowance.DirectBanking2Amount = cryptoService.Decrypt(allowance.DirectBanking2Amount);
            allowance.CashAmount = cryptoService.Decrypt(allowance.CashAmount);
            allowance.TotalNetAmount = cryptoService.Decrypt(allowance.TotalNetAmount);
            allowance.DSTNetSalaryAmount = cryptoService.Decrypt(allowance.DSTNetSalaryAmount);
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
                        addition.NetAllowanceToDisplay = RoundAmountWithCurrency(decimal.Parse(addition.NetAllowance), addition.CurrencyId.Value, 0).ToString(CultureInfo.InvariantCulture);
                    }

                    if (!string.IsNullOrEmpty(addition.TotalAmount))
                    {
                        addition.TotalAmountToDisplay = RoundAmountWithCurrency(decimal.Parse(addition.TotalAmount), addition.CurrencyId.Value).ToString(CultureInfo.InvariantCulture);
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
                        deduction.NetAllowanceToDisplay = RoundAmountWithCurrency(decimal.Parse(deduction.NetAllowance), deduction.CurrencyId.Value, 0).ToString(CultureInfo.InvariantCulture);
                    }

                    if (!string.IsNullOrEmpty(deduction.TotalAmount))
                    {
                        deduction.TotalAmountToDisplay = RoundAmountWithCurrency(decimal.Parse(deduction.TotalAmount), deduction.CurrencyId.Value).ToString(CultureInfo.InvariantCulture);
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

        private void DecryptAndRoudingAmountMonthlySalary(EditMonthlySalaryViewModel viewModel)
        {
            var currencyId = viewModel.MonthlySalary.CurrencyId ?? 0;
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
                ? RoundAmountWithCurrency(amount, currencyId).ToString(CultureInfo.InvariantCulture) : string.Empty;
            decimal.TryParse(viewModel.MonthlySalary.DeductionAmount, out amount);
            viewModel.MonthlySalary.DeductionAmountDisplay = !string.IsNullOrWhiteSpace(viewModel.MonthlySalary.DeductionAmount)
                ? RoundAmountWithCurrency(amount, currencyId).ToString(CultureInfo.InvariantCulture) : string.Empty;

            foreach (MonthlySalaryAdditionModel addition in viewModel.Additions)
            {
                addition.UnitAmount = cryptoService.Decrypt(addition.UnitAmount);
                addition.NetSalary = cryptoService.Decrypt(addition.NetSalary);

                decimal.TryParse(addition.NetSalary, out amount);
                addition.NetSalaryDisplay = !string.IsNullOrWhiteSpace(addition.NetSalary)
                    ? RoundAmountWithCurrency(amount, currencyId, 0).ToString(CultureInfo.InvariantCulture) : string.Empty;
            }

            foreach (MonthlySalaryDeductionModel deduction in viewModel.Deductions)
            {
                deduction.UnitAmount = cryptoService.Decrypt(deduction.UnitAmount);
                deduction.NetSalary = cryptoService.Decrypt(deduction.NetSalary);

                decimal.TryParse(deduction.NetSalary, out amount);
                deduction.NetSalaryDisplay = !string.IsNullOrWhiteSpace(deduction.NetSalary)
                    ? RoundAmountWithCurrency(amount, viewModel.MonthlySalary.CurrencyId ?? 0, 0).ToString(CultureInfo.InvariantCulture) : string.Empty;
            }
        }

        private static decimal RoundAmountWithCurrency(decimal amount, int currencyId, int digits = 2)
        {
            if (currencyId == Constants.VietnamCurrencyId)
            {
                return Math.Round(amount / 1000, MidpointRounding.AwayFromZero) * 1000;
            }

            return Math.Round(amount, digits, MidpointRounding.AwayFromZero);
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
            var currencyId = monthlyPayrollBasicAllowance.CurrencyId ?? 0;
            var directbanking1CurrencyId = monthlyPayrollBasicAllowance.DirectBanking1CurrencyId ?? 0;
            var directbanking2CurrencyId = monthlyPayrollBasicAllowance.DirectBanking2CurrencyId ?? 0;
            var cashCurrencyId = monthlyPayrollBasicAllowance.CashCurrencyId ?? 0;

            var directBanking1 = ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.DirectBanking1Amount, directbanking1CurrencyId, currencyId);
            var directBanking2 = ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.DirectBanking2Amount, directbanking2CurrencyId, currencyId);
            var cash = ConvertAmount(currencyExchangeRates, monthlyPayrollBasicAllowance.CashAmount, cashCurrencyId, currencyId);

            switch (monthlyPayrollBasicAllowance.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.DirectBanking1:
                    directBanking1 = RoundAmountWithCurrency(ConvertAmount(currencyExchangeRates, totalAmount - directBanking2 - cash, currencyId, directbanking1CurrencyId),
                        directbanking1CurrencyId, 0);
                    monthlyPayrollBasicAllowance.DirectBanking1Amount = directBanking1.ToString(CultureInfo.InvariantCulture);
                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    directBanking2 = RoundAmountWithCurrency(ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - cash, currencyId, directbanking2CurrencyId),
                        directbanking2CurrencyId, 0);
                    monthlyPayrollBasicAllowance.DirectBanking2Amount = directBanking2.ToString(CultureInfo.InvariantCulture);
                    break;

                case PayrollAutoCalculateOn.Cash:
                    cash = RoundAmountWithCurrency(ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - directBanking2, currencyId, cashCurrencyId), cashCurrencyId, 0);
                    monthlyPayrollBasicAllowance.CashAmount = cash.ToString(CultureInfo.InvariantCulture);
                    break;

                default:
                    return;
            }
        }

        private static void CalculateAutoAllowance(IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates, MonthlyAllowanceModel monthlyAllowance)
        {
            decimal.TryParse(monthlyAllowance.TotalAmount, out decimal totalAmount);
            var currencyId = monthlyAllowance.CurrencyId ?? 0;
            var directbanking1CurrencyId = monthlyAllowance.DirectBanking1CurrencyId ?? 0;
            var directbanking2CurrencyId = monthlyAllowance.DirectBanking2CurrencyId ?? 0;
            var cashCurrencyId = monthlyAllowance.CashCurrencyId ?? 0;

            var directBanking1 = ConvertAmount(currencyExchangeRates, monthlyAllowance.DirectBanking1Amount, directbanking1CurrencyId, currencyId);
            var directBanking2 = ConvertAmount(currencyExchangeRates, monthlyAllowance.DirectBanking2Amount, directbanking2CurrencyId, currencyId);
            var cash = ConvertAmount(currencyExchangeRates, monthlyAllowance.CashAmount, cashCurrencyId, currencyId);
            decimal autoAmount;

            switch (monthlyAllowance.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.DirectBanking1:
                    autoAmount = RoundAmountWithCurrency(ConvertAmount(currencyExchangeRates, totalAmount - directBanking2 - cash, currencyId, directbanking1CurrencyId), directbanking1CurrencyId, 0);
                    monthlyAllowance.DirectBanking1Amount = autoAmount.ToString(CultureInfo.InvariantCulture);
                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    autoAmount = RoundAmountWithCurrency(ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - cash, currencyId, directbanking2CurrencyId), directbanking2CurrencyId, 0);
                    monthlyAllowance.DirectBanking2Amount = autoAmount.ToString(CultureInfo.InvariantCulture);
                    break;

                case PayrollAutoCalculateOn.Cash:
                    autoAmount = RoundAmountWithCurrency(ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - directBanking2, currencyId, cashCurrencyId), cashCurrencyId, 0);
                    monthlyAllowance.CashAmount = autoAmount.ToString(CultureInfo.InvariantCulture);
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
                    ConvertAmount(currencyExchangeRates, monthlyAllowance.DSTNetSalaryAmount, monthlyAllowance.DSTNetSalaryCurrencyId.Value, monthlyAllowance.CurrencyId.Value);

                monthlyAllowance.TotalNetAmount = RoundAmountWithCurrency(totalNetAmount, monthlyAllowance.CurrencyId.Value, 0).ToString(CultureInfo.InvariantCulture);
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

        private static void CalculateBackPayAndAfterAdjustmentInCaseSalaryChangeReason(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            MonthlySalaryAdditionModel backPay,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            var currencyId = basicSetting.CurrencyId ?? 0;
            decimal.TryParse(basicSetting.OldSalary, out decimal oldSalary);
            decimal.TryParse(basicSetting.Salary, out decimal newSalary);
            oldSalary = oldSalary * GetExchangeRate(currencyExchangeRates, basicSetting.OldCurrencyId, currencyId);
            int daysInMonth = DateTime.DaysInMonth(workingMonth.Year, workingMonth.Month);
            int startDay = GetDifferenceMonth(groupStartDate, workingMonth) == 0 ? groupStartDate.Day : 1;
            int endDay = (groupEndDate.HasValue && GetDifferenceMonth(groupEndDate.Value, workingMonth) == 0) ? groupEndDate.Value.Day : daysInMonth;
            decimal amount;

            if (basicSetting.SalaryChangeReason == BasicSalaryChangeReason.AnnualIncrement)
            {
                CalculateBackPayAndAfterAdjustmentInCaseAnnualIncrement(currencyExchangeRates, workingMonth, basicSetting, backPay, groupStartDate, groupEndDate);
            }
            else
            {
                if (basicSetting.EffectiveDate.HasValue
                    && GetDifferenceMonth(basicSetting.EffectiveDate.Value, workingMonth) == 0
                    && oldSalary != newSalary)
                {
                    amount = (oldSalary / daysInMonth * (basicSetting.EffectiveDate.Value.Day - startDay))
                        + (newSalary / daysInMonth * (endDay - basicSetting.EffectiveDate.Value.Day + 1));
                    amount = RoundAmountWithCurrency(amount, currencyId);
                    basicSetting.AfterAdjustmentAmount = amount.ToString(CultureInfo.InvariantCulture);
                    GetAfterAdjustmentReason(workingMonth, basicSetting, groupStartDate, groupEndDate, amount);
                }
                else
                {
                    CalculateBackPayAndAfterAdjustmentInCaseGroupDateCommencement(workingMonth, basicSetting, groupStartDate, groupEndDate);
                }
            }
        }

        private static void CalculateBackPayAndAfterAdjustmentInCaseAnnualIncrement(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            MonthlySalaryAdditionModel backPay,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            var currencyId = basicSetting.CurrencyId ?? 0;
            decimal.TryParse(basicSetting.OldSalary, out decimal oldSalary);
            oldSalary = oldSalary * GetExchangeRate(currencyExchangeRates, basicSetting.OldCurrencyId, currencyId);
            decimal.TryParse(basicSetting.Salary, out decimal newSalary);
            decimal amount;

            if (basicSetting.EffectiveDate.HasValue && oldSalary != newSalary)
            {
                int monthDiff = GetDifferenceMonth(basicSetting.EffectiveDate.Value, workingMonth);
                if (monthDiff > 0)
                {
                    int daysInMonth = DateTime.DaysInMonth(basicSetting.EffectiveDate.Value.Year, basicSetting.EffectiveDate.Value.Month);
                    amount = ((newSalary - oldSalary) / daysInMonth * (daysInMonth - basicSetting.EffectiveDate.Value.Day + 1))
                        + ((newSalary - oldSalary) * (monthDiff - 1));

                    basicSetting.AfterAdjustmentAmount = RoundAmountWithCurrency(newSalary, currencyId).ToString(CultureInfo.InvariantCulture);

                    amount = RoundAmountWithCurrency(amount, currencyId);
                    backPay.UnitAmount = backPay.NetSalary = amount.ToString(CultureInfo.InvariantCulture);
                    backPay.Description = string.Format(Message.BackPayDescriptionFormat, basicSetting.EffectiveDate.Value.ToString(Constants.DateFormat));
                }
                else
                {
                    int daysInMonth = DateTime.DaysInMonth(workingMonth.Year, workingMonth.Month);
                    int startDay = GetDifferenceMonth(groupStartDate, workingMonth) == 0 ? groupStartDate.Day : 1;
                    int endDay = (groupEndDate.HasValue && GetDifferenceMonth(groupEndDate.Value, workingMonth) == 0) ? groupEndDate.Value.Day : daysInMonth;
                    amount = (oldSalary / daysInMonth * (basicSetting.EffectiveDate.Value.Day - startDay))
                        + (newSalary / daysInMonth * (endDay - basicSetting.EffectiveDate.Value.Day + 1));
                    amount = RoundAmountWithCurrency(amount, currencyId);
                    basicSetting.AfterAdjustmentAmount = amount.ToString(CultureInfo.InvariantCulture);
                    GetAfterAdjustmentReason(workingMonth, basicSetting, groupStartDate, groupEndDate, amount);
                }
            }
            else
            {
                CalculateBackPayAndAfterAdjustmentInCaseGroupDateCommencement(workingMonth, basicSetting, groupStartDate, groupEndDate);
            }
        }

        private static void CalculateBackPayAndAfterAdjustmentInCaseGroupDateCommencement(
            DateTime workingMonth,
            MonthlyBasicSalaryModel basicSetting,
            DateTime groupStartDate,
            DateTime? groupEndDate)
        {
            decimal.TryParse(basicSetting.Salary, out decimal newSalary);
            int daysInMonth = DateTime.DaysInMonth(workingMonth.Year, workingMonth.Month);
            int startDay = GetDifferenceMonth(groupStartDate, workingMonth) == 0 ? groupStartDate.Day : 1;
            int endDay = (groupEndDate.HasValue && GetDifferenceMonth(groupEndDate.Value, workingMonth) == 0) ? groupEndDate.Value.Day : daysInMonth;

            decimal amount = newSalary / daysInMonth * (endDay - startDay + 1);
            amount = RoundAmountWithCurrency(amount, basicSetting.CurrencyId ?? 0);
            basicSetting.AfterAdjustmentAmount = amount.ToString(CultureInfo.InvariantCulture);
            GetAfterAdjustmentReason(workingMonth, basicSetting, groupStartDate, groupEndDate, amount);
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
            decimal differenceAmount = RoundAmountWithCurrency(afterAdjustmentAmount - salary, basicSetting.CurrencyId ?? 0);
            var reason = new StringBuilder();
            if (differenceAmount != 0)
            {
                reason.AppendFormat(
                    "{0} {1} ({2}).",
                    differenceAmount > 0 ? "Added" : "Deducted",
                    Math.Abs(differenceAmount),
                    basicSetting.CurrencyCode);

                string dateCommencementReason = GetPartOfAfterAdjustmentReason(workingMonth, groupStartDate, groupEndDate);

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