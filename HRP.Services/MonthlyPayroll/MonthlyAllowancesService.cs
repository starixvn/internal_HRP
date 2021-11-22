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
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.PaymentProcess;
    using HRP.Models.ServiceModels;
    using HRP.Repositories;

    public class MonthlyAllowancesService : IMonthlyAllowancesService
    {
        private readonly IBaseService baseService;
        private readonly ICryptoService cryptoService;
        private readonly IMonthlyPayrollRepository monthlyPayrollRepository;

        public MonthlyAllowancesService(
            IBaseService baseService,
            ICryptoService cryptoService,
            IMonthlyPayrollRepository monthlyPayrollRepository)
        {
            this.baseService = baseService;
            this.cryptoService = cryptoService;
            this.monthlyPayrollRepository = monthlyPayrollRepository;
        }

        public async Task<MonthlyAllowancesModel> GetMonthlyAllowancesAsync(int payrollId, DateTime month)
        {
            var criteria = new GetMonthlyAllowanceCriteria { PayrollId = payrollId };
            (var allowance, var additions, var deductions, var basicAllowance)
                = await monthlyPayrollRepository.GetMonthlyAllowancesAsync(criteria);
            var model = new MonthlyAllowancesModel
            {
                Month = month,
                Allowance = allowance ?? new MonthlyAllowanceModel(),
                Additions = additions.ToList(),
                Deductions = deductions.ToList(),
                BasicAllowance = basicAllowance ?? new MonthlyPayrollBasicAllowanceModel()
            };

            DecryptMonthlyBasicAllowance(model.BasicAllowance);
            DecryptMonthlyAllowance(model.Allowance);
            DecryptMonthlyAllowanceAdditions(model.Additions, model.Allowance.CurrencyCode);
            DecryptMonthlyAllowanceDeductions(model.Deductions, model.Allowance.CurrencyCode);

            return model;
        }

        public async Task<CommandResult> EditMonthlyAllowancesAsync(MonthlyAllowancesModel model, int userId)
        {
            EncryptMonthlyAllowanceAddition(model.Additions);
            EncryptMonthlyAllowanceDeduction(model.Deductions);
            decimal.TryParse(model.Allowance.DirectBanking1Amount, out decimal directbanking1);
            decimal.TryParse(model.Allowance.DirectBanking2Amount, out decimal directbanking2);
            decimal.TryParse(model.Allowance.CashAmount, out decimal cash);
            var status = directbanking1 < 0 || directbanking2 < 0 || cash < 0 ? MonthlyPayrollStatus.Unsatisfied : MonthlyPayrollStatus.Satisfied;

            var criteria = new EditMonthlyAllowancesCommand
            {
                PayrollId = model.Allowance.PayrollId,
                CurrencyId = model.Allowance.CurrencyId,
                TotalAmount = cryptoService.Encrypt(model.Allowance.TotalAmount),
                DirectBanking1CurrencyId = model.Allowance.DirectBanking1CurrencyId,
                DirectBanking1Amount = cryptoService.Encrypt(model.Allowance.DirectBanking1Amount),
                DirectBanking1AccountId = model.Allowance.DirectBanking1AccountId,
                DirectBanking2CurrencyId = model.Allowance.DirectBanking2CurrencyId,
                DirectBanking2Amount = cryptoService.Encrypt(model.Allowance.DirectBanking2Amount),
                DirectBanking2AccountId = model.Allowance.DirectBanking2AccountId,
                CashCurrencyId = model.Allowance.CashCurrencyId,
                CashAmount = cryptoService.Encrypt(model.Allowance.CashAmount),
                CashAccountId = model.Allowance.CashAccountId,
                AutoCalculateOn = model.Allowance.AutoCalculateOn,
                TotalNetAmount = cryptoService.Encrypt(model.Allowance.TotalNetAmount),
                DSTNetSalaryCurrencyId = model.Allowance.DSTNetSalaryCurrencyId,
                DSTNetSalaryAmount = cryptoService.Encrypt(model.Allowance.DSTNetSalaryAmount),
                AdditionAmount = cryptoService.Encrypt(model.Allowance.AdditionAmount),
                DeductionAmount = cryptoService.Encrypt(model.Allowance.DeductionAmount),
                AdditionXml = model.Additions.ToXML().ToString(),
                DeductionXml = model.Deductions.ToXML().ToString(),
                UserId = userId,
                Status = status
            };

            return await baseService.ExecuteNonQueryAsync(criteria);
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
                    totalAmount -= roomingCharge;
                }

                if (basicAllowance.FitnessFirstCurrencyId.HasValue && !string.IsNullOrEmpty(basicAllowance.FitnessFirstAmount))
                {
                    var fitnessFirst = CalculationService.ConvertAmount(currencyExchangeRates, basicAllowance.FitnessFirstAmount,
                        basicAllowance.FitnessFirstCurrencyId.Value, basicAllowance.CurrencyId.Value);
                    totalAmount -= fitnessFirst;
                }

                var additionAmount = GetAdditionAmount(allowanceAdditions);
                var deductionAmount = GetDeductionAmount(allowanceDeductions);

                totalAmount += additionAmount;
                totalAmount -= deductionAmount;

                monthlyAllowance.AdditionAmount = CalculationService.RoundAmountWithCurrency(additionAmount, monthlyAllowance.CurrencyCode, 0).ToString(CultureInfo.InvariantCulture);
                monthlyAllowance.DeductionAmount = CalculationService.RoundAmountWithCurrency(deductionAmount, monthlyAllowance.CurrencyCode, 0).ToString(CultureInfo.InvariantCulture);
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

                monthlyPayrollBasicAllowance.TotalAmount = CalculationService.RoundAmountWithCurrency(totalAmount, monthlyPayrollBasicAllowance.CurrencyCode, 0)
                    .ToString(CultureInfo.InvariantCulture);

                CalculateAfterAdjustment(totalAmount, monthlyPayrollBasicAllowance, workingMonth, groupStartDate, groupEndDate, currencyExchangeRates);
                CalculateAutoBasicAllowance(currencyExchangeRates, monthlyPayrollBasicAllowance);
            }
        }

        public IEnumerable<MonthlyAllowanceAdditionModel> InitializeAllowanceAdditions(
            int standardWorkingDays,
            decimal actualWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            GeneratePayrollModel data)
        {
            var additions = Enum.GetValues(typeof(MonthlyAllowanceAdditionType)).Cast<MonthlyAllowanceAdditionType>()
                .Select(x => new MonthlyAllowanceAdditionModel
                {
                    PayrollId = basicAllowance.PayrollId,
                    EmployeeId = basicAllowance.EmployeeId,
                    Type = (byte)x,
                    CurrencyId = GetCurrencyBaseOnAllowanceType(x, basicAllowance)
                }).ToList();
            var additionDeductionUpload = data.AdditionDeductions.FirstOrDefault(x => x.EmployeeId == basicAllowance.EmployeeId);
            if (additionDeductionUpload != null)
            {
                MappingAllowanceAddition(
                    standardWorkingDays, basicAllowance, additions,
                    additionDeductionUpload, data.Currencies,
                    data.CurrencyExchangeRate);
            }

            if ((additionDeductionUpload == null
                    || string.IsNullOrWhiteSpace(additionDeductionUpload.Allowance_Addition_DutyMeal_CurrencyCode)
                    || string.IsNullOrWhiteSpace(additionDeductionUpload.Allowance_Addition_DutyMeal_NoOfDays))
                && standardWorkingDays < actualWorkingDays)
            {
                var dutyMeal = additions.FirstOrDefault(x => x.Type == (byte)MonthlyAllowanceAdditionType.DutyMeal);
                dutyMeal.NoOfDays = actualWorkingDays - standardWorkingDays;
                decimal.TryParse(basicAllowance.DutyMealAmount, out decimal dutyMealAmount);

                var unitAmount = dutyMealAmount / standardWorkingDays;
                var totalAmount = unitAmount * dutyMeal.NoOfDays;
                dutyMeal.UnitAmount = unitAmount.ToString(CultureInfo.InvariantCulture);
                dutyMeal.TotalAmount = totalAmount.ToString();

                if (dutyMeal.CurrencyId.HasValue && basicAllowance.CurrencyId.HasValue)
                {
                    var netAllowance = totalAmount.Value * CalculationService.GetExchangeRate(data.CurrencyExchangeRate, dutyMeal.CurrencyId.Value, basicAllowance.CurrencyId.Value);
                    dutyMeal.NetAllowance = netAllowance.ToString(CultureInfo.InvariantCulture);
                }
            }

            return additions;
        }

        public IEnumerable<MonthlyAllowanceDeductionModel> InitializeAllowanceDeductions(
            int standardWorkingDays,
            decimal actualWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            GeneratePayrollModel data)
        {
            var deductions = Enum.GetValues(typeof(MonthlyAllowanceAdditionType)).Cast<MonthlyAllowanceAdditionType>()
                .Select(x => new MonthlyAllowanceDeductionModel
                {
                    PayrollId = basicAllowance.PayrollId,
                    EmployeeId = basicAllowance.EmployeeId,
                    Type = (byte)x,
                    CurrencyId = GetCurrencyBaseOnAllowanceType(x, basicAllowance)
                }).ToList(); var additionDeductionUpload = data.AdditionDeductions.FirstOrDefault(x => x.EmployeeId == basicAllowance.EmployeeId);
            if (additionDeductionUpload != null)
            {
                MappingAllowanceDeduction(
                    standardWorkingDays, basicAllowance, deductions,
                    additionDeductionUpload, data.Currencies,
                    data.CurrencyExchangeRate);
            }

            if ((additionDeductionUpload == null
                    || string.IsNullOrWhiteSpace(additionDeductionUpload.Allowance_Deduction_DutyMeal_CurrencyCode)
                    || string.IsNullOrWhiteSpace(additionDeductionUpload.Allowance_Deduction_DutyMeal_NoOfDays))
                && standardWorkingDays > actualWorkingDays)
            {
                var dutyMeal = deductions.FirstOrDefault(x => x.Type == (byte)MonthlyAllowanceAdditionType.DutyMeal);
                dutyMeal.NoOfDays = standardWorkingDays - actualWorkingDays;
                decimal.TryParse(basicAllowance.DutyMealAmount, out decimal dutyMealAmount);

                var unitAmount = dutyMealAmount / standardWorkingDays;
                var totalAmount = unitAmount * dutyMeal.NoOfDays;
                dutyMeal.UnitAmount = unitAmount.ToString(CultureInfo.InvariantCulture);
                dutyMeal.TotalAmount = totalAmount.ToString();

                if (dutyMeal.CurrencyId.HasValue && basicAllowance.CurrencyId.HasValue)
                {
                    var netAllowance = totalAmount.Value * CalculationService.GetExchangeRate(data.CurrencyExchangeRate, dutyMeal.CurrencyId.Value, basicAllowance.CurrencyId.Value);
                    dutyMeal.NetAllowance = netAllowance.ToString(CultureInfo.InvariantCulture);
                }
            }

            return deductions;
        }

        private static decimal GetAdditionAmount(IEnumerable<MonthlyAllowanceAdditionModel> additions)
        {
            decimal additionAmount = 0;
            foreach (var addition in additions)
            {
                decimal.TryParse(addition.NetAllowance, out decimal amount);
                additionAmount += amount;
            }

            return additionAmount;
        }

        private static decimal GetDeductionAmount(IEnumerable<MonthlyAllowanceDeductionModel> deductions)
        {
            decimal deductionAmount = 0;
            foreach (var deduction in deductions)
            {
                decimal.TryParse(deduction.NetAllowance, out decimal amount);
                deductionAmount += amount;
            }

            return deductionAmount;
        }

#pragma warning disable S1541 // Methods and properties should not be too complex
        private static void MappingAllowanceAddition(
#pragma warning restore S1541 // Methods and properties should not be too complex
            int standardWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            IEnumerable<MonthlyAllowanceAdditionModel> additions,
            AdditionDeductionUploading additionDeductionUpload,
            IEnumerable<Currency> currencies,
            IEnumerable<CurrencyExchangeRateModel> exchangeRates)
        {
            foreach (var addition in additions)
            {
                string currencyCode;
                string amount;
                string noOfDays;
                switch (addition.Type)
                {
                    case (byte)MonthlyAllowanceAdditionType.Subsistence:
                        currencyCode = additionDeductionUpload.Allowance_Addition_Subsistence_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Addition_Subsistence_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Addition_Subsistence_NoOfDays;
                        MappingAllowanceAddition(basicAllowance.CurrencyId, addition, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.HomePassage:
                        currencyCode = additionDeductionUpload.Allowance_Addition_HomePassage_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Addition_HomePassage_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Addition_HomePassage_NoOfDays;
                        MappingAllowanceAddition(basicAllowance.CurrencyId, addition, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.Housing:
                        currencyCode = additionDeductionUpload.Allowance_Addition_Housing_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Addition_Housing_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Addition_Housing_NoOfDays;
                        MappingAllowanceAddition(basicAllowance.CurrencyId, addition, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.MobilePhone:
                        currencyCode = additionDeductionUpload.Allowance_Addition_MobilePhone_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Addition_MobilePhone_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Addition_MobilePhone_NoOfDays;
                        MappingAllowanceAddition(basicAllowance.CurrencyId, addition, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.HPI:
                        currencyCode = additionDeductionUpload.Allowance_Addition_HPI_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Addition_HPI_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Addition_HPI_NoOfDays;
                        MappingAllowanceAddition(basicAllowance.CurrencyId, addition, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.DutyMeal:
                        currencyCode = additionDeductionUpload.Allowance_Addition_DutyMeal_CurrencyCode;
                        noOfDays = additionDeductionUpload.Allowance_Addition_DutyMeal_NoOfDays;
                        MappingDutyMealAllowanceAddition(standardWorkingDays, basicAllowance, addition, currencyCode, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.Secondment:
                        currencyCode = additionDeductionUpload.Allowance_Addition_Secondment_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Addition_Secondment_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Addition_Secondment_NoOfDays;
                        MappingAllowanceAddition(basicAllowance.CurrencyId, addition, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.SpecialAllowances:
                        currencyCode = additionDeductionUpload.Allowance_Addition_SpecialAllowance_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Addition_SpecialAllowance_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Addition_SpecialAllowance_NoOfDays;
                        MappingAllowanceAddition(basicAllowance.CurrencyId, addition, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.OT:
                        currencyCode = additionDeductionUpload.Allowance_Addition_OT_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Addition_OT_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Addition_OT_NoOfDays;
                        MappingAllowanceAddition(basicAllowance.CurrencyId, addition, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.Others:
                        currencyCode = additionDeductionUpload.Allowance_Addition_Others_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Addition_Others_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Addition_Others_NoOfDays;
                        MappingAllowanceAddition(basicAllowance.CurrencyId, addition, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    default:
                        // do nothing
                        break;
                }
            }
        }

        private static void MappingAllowanceAddition(
            int? basicAllowanceCurrencyId,
            MonthlyAllowanceAdditionModel addition,
            string currencyCode,
            string amount,
            string noOfDays,
            IEnumerable<Currency> currencies,
            IEnumerable<CurrencyExchangeRateModel> exchangeRates)
        {
            var currency = currencies.FirstOrDefault(x => x.CurrencyCode.ToLower(CultureInfo.InvariantCulture).Equals(currencyCode?.ToLower(CultureInfo.InvariantCulture)));
            decimal.TryParse(amount, out decimal tempAmount);
            decimal.TryParse(noOfDays, out decimal tempNoOfDays);
            tempAmount = Math.Round(tempAmount, Constants.DefaultNoOfFractionalDigits, MidpointRounding.AwayFromZero);
            var totalAmount = tempAmount * tempNoOfDays;
            addition.NoOfDays = tempNoOfDays > 0 ? tempNoOfDays : (decimal?)null;
            addition.UnitAmount = tempAmount > 0 ? tempAmount.ToString(CultureInfo.InvariantCulture) : string.Empty;
            addition.TotalAmount = totalAmount > 0 ? totalAmount.ToString(CultureInfo.InvariantCulture) : string.Empty;
            addition.CurrencyId = currency?.CurrencyId;

            if (addition.CurrencyId.HasValue && basicAllowanceCurrencyId.HasValue)
            {
                var netAllowance = totalAmount * CalculationService.GetExchangeRate(exchangeRates, addition.CurrencyId.Value, basicAllowanceCurrencyId.Value);
                addition.NetAllowance = netAllowance > 0 ? netAllowance.ToString(CultureInfo.InvariantCulture) : string.Empty;
            }
        }

        private static void MappingDutyMealAllowanceAddition(
            int standardWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            MonthlyAllowanceAdditionModel addition,
            string currencyCode,
            string noOfDays,
            IEnumerable<Currency> currencies,
            IEnumerable<CurrencyExchangeRateModel> exchangeRates)
        {
            decimal.TryParse(noOfDays, out decimal tempNoOfDays);
            decimal.TryParse(basicAllowance.DutyMealAmount, out decimal dutyMealAmount);
            var currency = currencies.FirstOrDefault(x => x.CurrencyCode.ToLower(CultureInfo.InvariantCulture).Equals(currencyCode?.ToLower(CultureInfo.InvariantCulture)));
            var unitAmount = dutyMealAmount / standardWorkingDays;
            var totalAmount = unitAmount * tempNoOfDays;
            addition.NoOfDays = tempNoOfDays > 0 ? tempNoOfDays : (decimal?)null;
            addition.UnitAmount = unitAmount > 0 ? unitAmount.ToString(CultureInfo.InvariantCulture) : string.Empty;
            addition.TotalAmount = totalAmount > 0 ? totalAmount.ToString(CultureInfo.InvariantCulture) : string.Empty;
            addition.CurrencyId = currency?.CurrencyId;

            if (addition.CurrencyId.HasValue && basicAllowance.CurrencyId.HasValue)
            {
                var netAllowance = totalAmount * CalculationService.GetExchangeRate(exchangeRates, addition.CurrencyId.Value, basicAllowance.CurrencyId.Value);
                addition.NetAllowance = netAllowance > 0 ? netAllowance.ToString(CultureInfo.InvariantCulture) : string.Empty;
            }
        }

        private static void MappingAllowanceDeduction(
            int standardWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            IEnumerable<MonthlyAllowanceDeductionModel> deductions,
            AdditionDeductionUploading additionDeductionUpload,
            IEnumerable<Currency> currencies,
            IEnumerable<CurrencyExchangeRateModel> exchangeRates)
        {
            foreach (var deduction in deductions)
            {
                string currencyCode;
                string amount;
                string noOfDays;
                switch (deduction.Type)
                {
                    case (byte)MonthlyAllowanceAdditionType.Subsistence:
                        currencyCode = additionDeductionUpload.Allowance_Deduction_Subsistence_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Deduction_Subsistence_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Deduction_Subsistence_NoOfDays;
                        MappingAllowanceDeduction(basicAllowance.CurrencyId, deduction, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.HomePassage:
                        currencyCode = additionDeductionUpload.Allowance_Deduction_HomePassage_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Deduction_HomePassage_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Deduction_HomePassage_NoOfDays;
                        MappingAllowanceDeduction(basicAllowance.CurrencyId, deduction, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.Housing:
                        currencyCode = additionDeductionUpload.Allowance_Deduction_Housing_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Deduction_Housing_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Deduction_Housing_NoOfDays;
                        MappingAllowanceDeduction(basicAllowance.CurrencyId, deduction, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.MobilePhone:
                        currencyCode = additionDeductionUpload.Allowance_Deduction_MobilePhone_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Deduction_MobilePhone_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Deduction_MobilePhone_NoOfDays;
                        MappingAllowanceDeduction(basicAllowance.CurrencyId, deduction, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.HPI:
                        currencyCode = additionDeductionUpload.Allowance_Deduction_HPI_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Deduction_HPI_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Deduction_HPI_NoOfDays;
                        MappingAllowanceDeduction(basicAllowance.CurrencyId, deduction, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.DutyMeal:
                        currencyCode = additionDeductionUpload.Allowance_Deduction_DutyMeal_CurrencyCode;
                        noOfDays = additionDeductionUpload.Allowance_Deduction_DutyMeal_NoOfDays;
                        MappingDutyMealAllowanceDeduction(standardWorkingDays, basicAllowance, deduction, currencyCode, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.Secondment:
                        currencyCode = additionDeductionUpload.Allowance_Deduction_Secondment_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Deduction_Secondment_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Deduction_Secondment_NoOfDays;
                        MappingAllowanceDeduction(basicAllowance.CurrencyId, deduction, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    case (byte)MonthlyAllowanceAdditionType.Others:
                        currencyCode = additionDeductionUpload.Allowance_Deduction_Others_CurrencyCode;
                        amount = additionDeductionUpload.Allowance_Deduction_Others_Amount;
                        noOfDays = additionDeductionUpload.Allowance_Deduction_Others_NoOfDays;
                        MappingAllowanceDeduction(basicAllowance.CurrencyId, deduction, currencyCode, amount, noOfDays, currencies, exchangeRates);
                        break;

                    default:
                        // do nothing
                        break;
                }
            }
        }

        private static void MappingAllowanceDeduction(
            int? basicAllowanceCurrencyId,
            MonthlyAllowanceDeductionModel deduction,
            string currencyCode,
            string amount,
            string noOfDays,
            IEnumerable<Currency> currencies,
            IEnumerable<CurrencyExchangeRateModel> exchangeRates)
        {
            var currency = currencies.FirstOrDefault(x => x.CurrencyCode.ToLower(CultureInfo.InvariantCulture).Equals(currencyCode?.ToLower(CultureInfo.InvariantCulture)));
            decimal.TryParse(amount, out decimal tempAmount);
            decimal.TryParse(noOfDays, out decimal tempNoOfDays);
            tempAmount = Math.Round(tempAmount, Constants.DefaultNoOfFractionalDigits, MidpointRounding.AwayFromZero);
            var totalAmount = tempAmount * tempNoOfDays;
            deduction.NoOfDays = tempNoOfDays > 0 ? tempNoOfDays : (decimal?)null;
            deduction.UnitAmount = tempAmount > 0 ? tempAmount.ToString(CultureInfo.InvariantCulture) : string.Empty;
            deduction.TotalAmount = totalAmount > 0 ? totalAmount.ToString(CultureInfo.InvariantCulture) : string.Empty;
            deduction.CurrencyId = currency?.CurrencyId;

            if (deduction.CurrencyId.HasValue && basicAllowanceCurrencyId.HasValue)
            {
                var netAllowance = totalAmount * CalculationService.GetExchangeRate(exchangeRates, deduction.CurrencyId.Value, basicAllowanceCurrencyId.Value);
                deduction.NetAllowance = netAllowance > 0 ? netAllowance.ToString(CultureInfo.InvariantCulture) : string.Empty;
            }
        }

        private static void MappingDutyMealAllowanceDeduction(
            int standardWorkingDays,
            MonthlyPayrollBasicAllowanceModel basicAllowance,
            MonthlyAllowanceDeductionModel deduction,
            string currencyCode,
            string noOfDays,
            IEnumerable<Currency> currencies,
            IEnumerable<CurrencyExchangeRateModel> exchangeRates)
        {
            decimal.TryParse(noOfDays, out decimal tempNoOfDays);
            decimal.TryParse(basicAllowance.DutyMealAmount, out decimal dutyMealAmount);
            var currency = currencies.FirstOrDefault(x => x.CurrencyCode.ToLower(CultureInfo.InvariantCulture).Equals(currencyCode?.ToLower(CultureInfo.InvariantCulture)));
            var unitAmount = dutyMealAmount / standardWorkingDays;
            var totalAmount = unitAmount * tempNoOfDays;
            deduction.NoOfDays = tempNoOfDays > 0 ? tempNoOfDays : (decimal?)null;
            deduction.UnitAmount = unitAmount > 0 ? unitAmount.ToString(CultureInfo.InvariantCulture) : string.Empty;
            deduction.TotalAmount = totalAmount > 0 ? totalAmount.ToString(CultureInfo.InvariantCulture) : string.Empty;
            deduction.CurrencyId = currency?.CurrencyId;

            if (deduction.CurrencyId.HasValue && basicAllowance.CurrencyId.HasValue)
            {
                var netAllowance = totalAmount * CalculationService.GetExchangeRate(exchangeRates, deduction.CurrencyId.Value, basicAllowance.CurrencyId.Value);
                deduction.NetAllowance = netAllowance > 0 ? netAllowance.ToString(CultureInfo.InvariantCulture) : string.Empty;
            }
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
            int startDay = CalculationService.GetDifferenceMonth(groupStartDate, workingMonth) == 0 ? groupStartDate.Day : 1;
            int endDay = (groupEndDate.HasValue && CalculationService.GetDifferenceMonth(groupEndDate.Value, workingMonth) == 0) ? groupEndDate.Value.Day : daysInMonth;
            var afterAdjustmentAmount = endDay == daysInMonth && startDay == 1 ? totalAmount : totalAmount / daysInMonth * (endDay - startDay + 1);
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
                string dateCommencementReason = CalculationService.GetPartOfAfterAdjustmentReason(workingMonth, groupStartDate, groupEndDate);

                reason.AppendFormat("{0} {1} ({2}). ", amount > 0 ? "Added" : "Deducted",
                    CalculationService.RoundAmountWithCurrency(Math.Abs(amount), monthlyPayrollBasicAllowance.CurrencyCode, roundingThousand: false),
                    monthlyPayrollBasicAllowance.CurrencyCode);
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
                afterAdjustmentAmount = CalculationService.RoundAmountWithCurrency(
                    CalculationService.ConvertAmount(currencyExchangeRate, afterAdjustmentAmount, allowanceDetailCurrencyId, monthlyPayrollBasicAllowance.CurrencyId),
                    monthlyPayrollBasicAllowance.CurrencyCode, roundingThousand: false);

                return afterAdjustmentAmount.ToString(CultureInfo.InvariantCulture);
            }

            return string.Empty;
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

            if (basicAllowance.CurrencyId.HasValue && !string.IsNullOrEmpty(basicAllowance.AfterAdjustmentAmount))
            {
                basicAllowance.AfterAdjustmentAmountDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(basicAllowance.AfterAdjustmentAmount),
                    basicAllowance.CurrencyCode, roundingThousand: false).ToStringWithCurrency(basicAllowance.CurrencyCode);
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

        private void DecryptMonthlyAllowanceAdditions(IEnumerable<MonthlyAllowanceAdditionModel> additions, string allowanceCurrencyCode)
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
                        addition.NetAllowanceToDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(addition.NetAllowance), allowanceCurrencyCode, 0)
                            .ToString(CultureInfo.InvariantCulture);
                    }

                    if (!string.IsNullOrEmpty(addition.TotalAmount))
                    {
                        addition.TotalAmountToDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(addition.TotalAmount), addition.CurrencyCode, roundingThousand: false)
                            .ToStringWithCurrency(addition.CurrencyCode);
                    }
                }
            }
        }

        private void DecryptMonthlyAllowanceDeductions(IEnumerable<MonthlyAllowanceDeductionModel> deductions, string allowanceCurrencyCode)
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
                        deduction.NetAllowanceToDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(deduction.NetAllowance), allowanceCurrencyCode, 0)
                            .ToString(CultureInfo.InvariantCulture);
                    }

                    if (!string.IsNullOrEmpty(deduction.TotalAmount))
                    {
                        deduction.TotalAmountToDisplay = CalculationService.RoundAmountWithCurrency(decimal.Parse(deduction.TotalAmount), deduction.CurrencyCode, roundingThousand: false)
                            .ToStringWithCurrency(deduction.CurrencyCode);
                    }
                }
            }
        }

        private void EncryptMonthlyAllowanceAddition(IEnumerable<MonthlyAllowanceAdditionModel> additions)
        {
            foreach (var addition in additions)
            {
                addition.UnitAmount = cryptoService.Encrypt(addition.UnitAmount);
                addition.TotalAmount = cryptoService.Encrypt(addition.TotalAmount);
                addition.NetAllowance = cryptoService.Encrypt(addition.NetAllowance);
            }
        }

        private void EncryptMonthlyAllowanceDeduction(IEnumerable<MonthlyAllowanceDeductionModel> deductions)
        {
            foreach (var deduction in deductions)
            {
                deduction.UnitAmount = cryptoService.Encrypt(deduction.UnitAmount);
                deduction.TotalAmount = cryptoService.Encrypt(deduction.TotalAmount);
                deduction.NetAllowance = cryptoService.Encrypt(deduction.NetAllowance);
            }
        }

        private static int? GetCurrencyBaseOnAllowanceType(MonthlyAllowanceAdditionType type, MonthlyPayrollBasicAllowanceModel basicAllowance)
        {
            switch (type)
            {
                case MonthlyAllowanceAdditionType.Subsistence: return basicAllowance.SubsistenceCurrencyId;
                case MonthlyAllowanceAdditionType.HomePassage: return basicAllowance.HomePassageCurrencyId;
                case MonthlyAllowanceAdditionType.Housing: return basicAllowance.HousingCurrencyId;
                case MonthlyAllowanceAdditionType.MobilePhone: return basicAllowance.MobilePhoneCurrencyId;
                case MonthlyAllowanceAdditionType.HPI: return basicAllowance.HPICurrencyId;
                case MonthlyAllowanceAdditionType.DutyMeal: return basicAllowance.DutyMealCurrencyId;
                case MonthlyAllowanceAdditionType.Secondment: return basicAllowance.SecondmentCurrencyId;
                case MonthlyAllowanceAdditionType.Others: return basicAllowance.OthersCurrencyId;
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
            decimal.TryParse(monthlyAllowance.TotalAmount, out decimal totalAmount);

            if (monthlyAllowance.DSTNetSalaryCurrencyId.HasValue && !string.IsNullOrEmpty(monthlyAllowance.DSTNetSalaryAmount))
            {
                var totalNetAmount = totalAmount -
                    CalculationService.ConvertAmount(currencyExchangeRates, monthlyAllowance.DSTNetSalaryAmount, monthlyAllowance.DSTNetSalaryCurrencyId.Value, monthlyAllowance.CurrencyId.Value);

                monthlyAllowance.TotalNetAmount = CalculationService.RoundAmountWithCurrency(totalNetAmount, monthlyAllowance.CurrencyCode, roundingThousand: false)
                    .ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                monthlyAllowance.TotalNetAmount = totalAmount.ToStringWithCurrency(monthlyAllowance.CurrencyCode);
            }
        }
    }
}