namespace HRP.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Constants;
    using Core.Services;
    using Criteria.Report;
    using HRP.Core.Enumerations;
    using HRP.Criteria;
    using Models.Currency;
    using Models.MonthlyPayroll;
    using Models.Report;
    using Repositories;

    public class ReportService : IReportService
    {
        private readonly ICryptoService cryptoService;
        private readonly IReportRepository reportRepository;
        private readonly ISessionService sessionService;
        private readonly IBaseService baseService;

        public ReportService(
            ICryptoService cryptoService,
            ISessionService sessionService,
            IBaseService baseService,
            IReportRepository reportRepository = null)
        {
            this.cryptoService = cryptoService;
            this.sessionService = sessionService;
            this.reportRepository = reportRepository ?? new ReportRepository();
            this.baseService = baseService;
        }

        public async Task<BankInReport> GetDirectBankingBankInAsync(
            int companyId,
            DateTime selectedTime,
            PaymentType typeOfPayment,
            string selectedPaymentTransactions)
        {
            var criteria = new GetDataForDirectBankingReportCriteria
            {
                UserId = sessionService.UserId,
                CompanyId = companyId,
                Month = selectedTime,
                PaymentTransactions = selectedPaymentTransactions,
                PaymentType = (int)typeOfPayment
            };

            var bankInData = await reportRepository.GetDataForBankInReportAsync(criteria).ConfigureAwait(false);

            if (bankInData != null)
            {
                DecryptBankInAmount(bankInData.EBankingBankIns);
                DecryptBankInAmount(bankInData.CashBankIns);
            }

            return bankInData;
        }

        public async Task<BankInReport> GetEbankingBankInAsync(int companyId, DateTime selectedTime, string selectedPaymentTransactions)
        {
            var criteria = new GetDataForEbankingReportCriteria
            {
                UserId = sessionService.UserId,
                CompanyId = companyId,
                Month = selectedTime,
                PaymentTransactions = selectedPaymentTransactions
            };

            var bankInData = await reportRepository.GetDataForBankInReportAsync(criteria).ConfigureAwait(false);

            DecryptBankInAmount(bankInData.EBankingBankIns);
            DecryptBankInAmount(bankInData.CashBankIns);

            return bankInData;
        }

        public IEnumerable<ColumnGroup> GetAvailableColumnsList()
        {
            return ColumnsConfirguration.Columns.Values.SelectMany(x => x).OrderBy(x => x.GroupName);
        }

        private void DecryptBankInAmount(IEnumerable<BankIn> bankIns)
        {
            if (bankIns != null && bankIns.Any())
            {
                foreach (BankIn bankIn in bankIns)
                {
                    float.TryParse(cryptoService.Decrypt(bankIn.Amount), out float parsedValue);
                    bankIn.AmountDecrypted = parsedValue;
                }
            }
        }

        #region MonthlyPayrollReport

        public async Task<MonthlyPayrollReport> GetDataForMonthlyPayrollReportAsync(DateTime month, int companyId)
        {
            var criteria = new GetDataForMonthlyPayrollReportCriteria
            {
                CompanyId = companyId,
                Month = month
            };

            var report = await reportRepository.GetDataForMonthlyPayrollReportAsync(criteria).ConfigureAwait(false);
            report.Month = month;

            var USDCurrencyId = report.CurrencyList?.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.USDCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId;
            var MYRCurrencyId = report.CurrencyList?.FirstOrDefault(x => string.Equals(x.CurrencyCode, Constants.MYRCurrencyCode, StringComparison.OrdinalIgnoreCase))?.CurrencyId;
            DecryptMonthlyBasicSalaryList(report.MonthlyBasicSalary);
            DecryptMonthlySalaryListAndConvertAmount(report.MonthlySalary, report.CurrencyExchangeRate, MYRCurrencyId, USDCurrencyId);
            DecryptMonthlyBasicAllowanceListAndConvertAmount(report.MonthlyBasicAllowance, report.CurrencyExchangeRate);
            DecryptMonthlyAllowanceListAndConvertAmount(report.MonthlyAllowance, report.CurrencyExchangeRate);

            return report;
        }

        private void DecryptMonthlyBasicSalaryList(IEnumerable<MonthlyBasicSalaryModel> basicSalaryList)
        {
            if (basicSalaryList != null && basicSalaryList.Any())
            {
                foreach (MonthlyBasicSalaryModel basicSalary in basicSalaryList)
                {
                    basicSalary.Salary = cryptoService.Decrypt(basicSalary.Salary);
                    basicSalary.AfterAdjustmentAmount = cryptoService.Decrypt(basicSalary.AfterAdjustmentAmount);
                }
            }
        }

        private void DecryptMonthlySalaryListAndConvertAmount(
            IEnumerable<MonthlySalaryModel> salaryList,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            int? MYRCurrencyId,
            int? USDCurrencyId)
        {
            if (salaryList != null && salaryList.Any())
            {
                foreach (MonthlySalaryModel salary in salaryList)
                {
                    salary.Salary = cryptoService.Decrypt(salary.Salary);
                    salary.EbankingAmount = cryptoService.Decrypt(salary.EbankingAmount);
                    salary.DirectBanking1Amount = cryptoService.Decrypt(salary.DirectBanking1Amount);
                    salary.DirectBanking2Amount = cryptoService.Decrypt(salary.DirectBanking2Amount);
                    salary.CashAmount = cryptoService.Decrypt(salary.CashAmount);
                    salary.AdditionAmount = cryptoService.Decrypt(salary.AdditionAmount);
                    salary.DeductionAmount = cryptoService.Decrypt(salary.DeductionAmount);

                    salary.SalaryAmountInMYR = ConvertAmount(currencyExchangeRates, salary.Salary, salary.CurrencyId, MYRCurrencyId);
                    salary.SalaryAmountInUSD = ConvertAmount(currencyExchangeRates, salary.Salary, salary.CurrencyId, USDCurrencyId);
                    salary.EbankingAmountInMYR = ConvertAmount(currencyExchangeRates, salary.EbankingAmount, salary.EbankingCurrencyId, MYRCurrencyId);
                    salary.DirectBanking1AmountInMYR = ConvertAmount(currencyExchangeRates, salary.DirectBanking1Amount, salary.DirectBanking1CurrencyId, MYRCurrencyId);
                    salary.DirectBanking2AmountInMYR = ConvertAmount(currencyExchangeRates, salary.DirectBanking2Amount, salary.DirectBanking2CurrencyId, MYRCurrencyId);
                    salary.CashAmountInMYR = ConvertAmount(currencyExchangeRates, salary.CashAmount, salary.CashCurrencyId, MYRCurrencyId);
                }
            }
        }

        private void DecryptMonthlyBasicAllowanceListAndConvertAmount(
            IEnumerable<MonthlyPayrollBasicAllowanceModel> basicAllowanceList,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            if (basicAllowanceList != null && basicAllowanceList.Any())
            {
                foreach (var basicAllowance in basicAllowanceList)
                {
                    basicAllowance.TotalAmount = cryptoService.Decrypt(basicAllowance.TotalAmount);
                    basicAllowance.SubsistenceAmount = cryptoService.Decrypt(basicAllowance.SubsistenceAmount);
                    basicAllowance.HomePassageAmount = cryptoService.Decrypt(basicAllowance.HomePassageAmount);
                    basicAllowance.HousingAmount = cryptoService.Decrypt(basicAllowance.HousingAmount);
                    basicAllowance.MobilePhoneAmount = cryptoService.Decrypt(basicAllowance.MobilePhoneAmount);
                    basicAllowance.HPIAmount = cryptoService.Decrypt(basicAllowance.HPIAmount);
                    basicAllowance.DutyMealAmount = cryptoService.Decrypt(basicAllowance.DutyMealAmount);
                    basicAllowance.SecondmentAmount = cryptoService.Decrypt(basicAllowance.SecondmentAmount);
                    basicAllowance.OthersAmount = cryptoService.Decrypt(basicAllowance.OthersAmount);
                    basicAllowance.RoomingChargesAmount = cryptoService.Decrypt(basicAllowance.RoomingChargesAmount);
                    basicAllowance.FitnessFirstAmount = cryptoService.Decrypt(basicAllowance.FitnessFirstAmount);
                    basicAllowance.AfterAdjustmentAmount = cryptoService.Decrypt(basicAllowance.AfterAdjustmentAmount);
                    basicAllowance.DutyMealAfterAdjustmentAmount = cryptoService.Decrypt(basicAllowance.DutyMealAfterAdjustmentAmount);
                    basicAllowance.HomePassageAfterAdjustmentAmount = cryptoService.Decrypt(basicAllowance.HomePassageAfterAdjustmentAmount);
                    basicAllowance.HousingAfterAdjustmentAmount = cryptoService.Decrypt(basicAllowance.HousingAfterAdjustmentAmount);
                    basicAllowance.HPIAfterAdjustmentAmount = cryptoService.Decrypt(basicAllowance.HPIAfterAdjustmentAmount);
                    basicAllowance.MobilePhoneAfterAdjustmentAmount = cryptoService.Decrypt(basicAllowance.MobilePhoneAfterAdjustmentAmount);
                    basicAllowance.OthersAfterAdjustmentAmount = cryptoService.Decrypt(basicAllowance.OthersAfterAdjustmentAmount);
                    basicAllowance.SecondmentAfterAdjustmentAmount = cryptoService.Decrypt(basicAllowance.SecondmentAfterAdjustmentAmount);
                    basicAllowance.SubsistenceAfterAdjustmentAmount = cryptoService.Decrypt(basicAllowance.SubsistenceAfterAdjustmentAmount);

                    ConvertMonthlyBasicAllowanceToConversionCurrency(basicAllowance, currencyExchangeRates);
                }
            }
        }

        private void DecryptMonthlyAllowanceListAndConvertAmount(
            IEnumerable<MonthlyAllowanceModel> allowanceList,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            if (allowanceList != null && allowanceList.Any())
            {
                foreach (var allowance in allowanceList)
                {
                    allowance.TotalAmount = cryptoService.Decrypt(allowance.TotalAmount);
                    allowance.DSTNetSalaryAmount = cryptoService.Decrypt(allowance.DSTNetSalaryAmount);
                    allowance.TotalNetAmount = cryptoService.Decrypt(allowance.TotalNetAmount);
                    allowance.AdditionAmount = cryptoService.Decrypt(allowance.AdditionAmount);
                    allowance.DeductionAmount = cryptoService.Decrypt(allowance.DeductionAmount);

                    ConvertMonthlyAllowanceToConversionCurrency(allowance, currencyExchangeRates);
                }
            }
        }

        private static void ConvertMonthlyBasicAllowanceToConversionCurrency(MonthlyPayrollBasicAllowanceModel basicAllowance, IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            var conversionCurrencyId = basicAllowance.CurrencyId;
            basicAllowance.SubsistenceAmount = ConvertAmount(currencyExchangeRates, basicAllowance.SubsistenceAmount,
                            basicAllowance.SubsistenceCurrencyId, conversionCurrencyId).ToString(CultureInfo.InvariantCulture);
            basicAllowance.HomePassageAmount = ConvertAmount(currencyExchangeRates, basicAllowance.HomePassageAmount,
                            basicAllowance.HomePassageCurrencyId, conversionCurrencyId).ToString(CultureInfo.InvariantCulture);
            basicAllowance.HousingAmount = ConvertAmount(currencyExchangeRates, basicAllowance.HousingAmount,
                            basicAllowance.HousingCurrencyId, conversionCurrencyId).ToString(CultureInfo.InvariantCulture);
            basicAllowance.MobilePhoneAmount = ConvertAmount(currencyExchangeRates, basicAllowance.MobilePhoneAmount,
                            basicAllowance.MobilePhoneCurrencyId, conversionCurrencyId).ToString(CultureInfo.InvariantCulture);
            basicAllowance.HPIAmount = ConvertAmount(currencyExchangeRates, basicAllowance.HPIAmount,
                            basicAllowance.HPICurrencyId, conversionCurrencyId).ToString(CultureInfo.InvariantCulture);
            basicAllowance.DutyMealAmount = ConvertAmount(currencyExchangeRates, basicAllowance.DutyMealAmount,
                            basicAllowance.DutyMealCurrencyId, conversionCurrencyId).ToString(CultureInfo.InvariantCulture);
            basicAllowance.SecondmentAmount = ConvertAmount(currencyExchangeRates, basicAllowance.SecondmentAmount,
                            basicAllowance.SecondmentCurrencyId, conversionCurrencyId).ToString(CultureInfo.InvariantCulture);
            basicAllowance.OthersAmount = ConvertAmount(currencyExchangeRates, basicAllowance.OthersAmount,
                            basicAllowance.OthersCurrencyId, conversionCurrencyId).ToString(CultureInfo.InvariantCulture);
            basicAllowance.RoomingChargesAmount = ConvertAmount(currencyExchangeRates, basicAllowance.RoomingChargesAmount,
                            basicAllowance.RoomingChargesCurrencyId, conversionCurrencyId).ToString(CultureInfo.InvariantCulture);
            basicAllowance.FitnessFirstAmount = ConvertAmount(currencyExchangeRates, basicAllowance.FitnessFirstAmount,
                            basicAllowance.FitnessFirstCurrencyId, conversionCurrencyId).ToString(CultureInfo.InvariantCulture);

            basicAllowance.SubsistenceCurrencyId = conversionCurrencyId;
            basicAllowance.HomePassageCurrencyId = conversionCurrencyId;
            basicAllowance.HousingCurrencyId = conversionCurrencyId;
            basicAllowance.MobilePhoneCurrencyId = conversionCurrencyId;
            basicAllowance.HPICurrencyId = conversionCurrencyId;
            basicAllowance.DutyMealCurrencyId = conversionCurrencyId;
            basicAllowance.SecondmentCurrencyId = conversionCurrencyId;
            basicAllowance.OthersCurrencyId = conversionCurrencyId;
            basicAllowance.RoomingChargesCurrencyId = conversionCurrencyId;
            basicAllowance.FitnessFirstCurrencyId = conversionCurrencyId;
        }

        private static void ConvertMonthlyAllowanceToConversionCurrency(
            MonthlyAllowanceModel allowance,
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            var conversionCurrencyId = allowance.CurrencyId;
            allowance.DSTNetSalaryAmount = ConvertAmount(currencyExchangeRates, allowance.DSTNetSalaryAmount,
                allowance.DSTNetSalaryCurrencyId, conversionCurrencyId).ToString(CultureInfo.InvariantCulture);
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

        private static decimal GetExchangeRate(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            int fromCurrencyId,
            int toCurrencyId)
        {
            CurrencyExchangeRateModel currencyExchangeRate =
                currencyExchangeRates.FirstOrDefault(x => x.FromCurrencyId == fromCurrencyId && x.ToCurrencyId == toCurrencyId);

            return currencyExchangeRate?.ExchangeRate ?? 0;
        }

        private static decimal RoundAmountWithCurrency(decimal amount, string currencyCode, int digits = 2)
        {
            if (string.Equals(currencyCode, Constants.VNDCurrencyCode, StringComparison.OrdinalIgnoreCase))
            {
                return Math.Round(amount / 1000, MidpointRounding.AwayFromZero) * 1000;
            }

            return Math.Round(amount, digits, MidpointRounding.AwayFromZero);
        }

        #endregion MonthlyPayrollReport

        public async Task<IEnumerable<DynamicReportModel>> GetEmployeeDataForDynamicReport(int companyId, string employeeStatusIds)
        {
            var employeeDatas = await baseService.GetDataAsync<DynamicReportModel>(new GetEmployeeDataForDynamicReportCriteria
            {
                CompanyId = companyId,
                EmployeeStatusIds = employeeStatusIds
            }).ConfigureAwait(false);

            var currencyExchangeRates = await baseService.GetDataAsync<CurrencyExchangeRateModel>(new GetLatestExchangeRatesCriteria()).ConfigureAwait(false);

            foreach (var data in employeeDatas)
            {
                DecryptEmployeeData(data);
                CalculateAutoSalary(data, currencyExchangeRates);
                CalculateAutoAllowance(data, currencyExchangeRates);
            }

            return employeeDatas;
        }

        private static void CalculateAutoAllowance(DynamicReportModel data, IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            if (!data.AllowanceCurrencyId.HasValue)
            {
                return;
            }

            var currencyId = data.AllowanceCurrencyId.Value;
            decimal totalAmount = 0;

            totalAmount += ConvertAmount(currencyExchangeRates, data.SubsistenceAmount, data.SubsistenceCurrencyId, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, data.HomePassageAmount, data.HomePassageCurrencyId, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, data.HousingAmount, data.HousingCurrencyId, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, data.MobilePhoneAmount, data.MobilePhoneCurrencyId, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, data.HPIAmount, data.HPICurrencyId, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, data.DutyMealAmount, data.DutyMealCurrencyId, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, data.SecondmentAmount, data.SecondmentCurrencyId, currencyId);

            totalAmount += ConvertAmount(currencyExchangeRates, data.OthersAmount, data.OthersCurrencyId, currencyId);

            totalAmount = RoundAmountWithCurrency(totalAmount, data.CurrencyCode, 0);
            data.AllowanceAmount = totalAmount.ToString(CultureInfo.InvariantCulture);

            var directBanking1 = ConvertAmount(currencyExchangeRates, data.AllowanceDirectBanking1Amount,
               data.AllowanceDirectBanking1CurrencyId, data.AllowanceCurrencyId);
            var directBanking2 = ConvertAmount(currencyExchangeRates, data.AllowanceDirectBanking2Amount,
                data.AllowanceDirectBanking2CurrencyId, data.AllowanceCurrencyId);
            var cash = ConvertAmount(currencyExchangeRates, data.AllowanceCashAmount, data.AllowanceCashCurrencyId, data.AllowanceCurrencyId);

            switch (data.AllowanceAutoCalculateOn)
            {
                case PayrollAutoCalculateOn.DirectBanking1:
                    if (data.DirectBanking1CurrencyId.HasValue)
                    {
                        directBanking1 = RoundAmountWithCurrency(
                            ConvertAmount(currencyExchangeRates, totalAmount - directBanking2 - cash, data.AllowanceCurrencyId, data.AllowanceDirectBanking1CurrencyId),
                            data.AllowanceDirectBanking1CurrencyCode, 0);

                        data.AllowanceDirectBanking1Amount = directBanking1.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    if (data.DirectBanking2CurrencyId.HasValue)
                    {
                        directBanking2 = RoundAmountWithCurrency(
                            ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - cash, data.AllowanceCurrencyId, data.AllowanceDirectBanking2CurrencyId),
                            data.AllowanceDirectBanking2CurrencyCode, 0);

                        data.AllowanceDirectBanking2Amount = directBanking2.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.Cash:
                    if (data.CashCurrencyId.HasValue)
                    {
                        cash = RoundAmountWithCurrency(
                            ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - directBanking2, data.AllowanceCurrencyId, data.AllowanceCashCurrencyId),
                            data.AllowanceCashCurrencyCode, 0);

                        data.AllowanceCashAmount = cash.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                default:
                    return;
            }
        }

        private static void CalculateAutoSalary(DynamicReportModel data, IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates)
        {
            decimal.TryParse(data.Salary, out decimal totalAmount);

            var directBanking1 = ConvertAmount(currencyExchangeRates, data.DirectBanking1Amount,
                data.DirectBanking1CurrencyId, data.CurrencyId);
            var directBanking2 = ConvertAmount(currencyExchangeRates, data.DirectBanking2Amount,
                data.DirectBanking2CurrencyId, data.CurrencyId);
            var cash = ConvertAmount(currencyExchangeRates, data.CashAmount, data.CashCurrencyId, data.CurrencyId);

            switch (data.AutoCalculateOn)
            {
                case PayrollAutoCalculateOn.DirectBanking1:
                    if (data.DirectBanking1CurrencyId.HasValue)
                    {
                        directBanking1 = RoundAmountWithCurrency(
                            ConvertAmount(currencyExchangeRates, totalAmount - directBanking2 - cash, data.CurrencyId, data.DirectBanking1CurrencyId),
                            data.DirectBanking1CurrencyCode, 0);

                        data.DirectBanking1Amount = directBanking1.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.DirectBanking2:
                    if (data.DirectBanking2CurrencyId.HasValue)
                    {
                        directBanking2 = RoundAmountWithCurrency(
                            ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - cash, data.CurrencyId, data.DirectBanking2CurrencyId),
                            data.DirectBanking2CurrencyCode, 0);

                        data.DirectBanking2Amount = directBanking2.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                case PayrollAutoCalculateOn.Cash:
                    if (data.CashCurrencyId.HasValue)
                    {
                        cash = RoundAmountWithCurrency(
                            ConvertAmount(currencyExchangeRates, totalAmount - directBanking1 - directBanking2, data.CurrencyId, data.CashCurrencyId),
                            data.CashCurrencyCode, 0);

                        data.CashAmount = cash.ToString(CultureInfo.InvariantCulture);
                    }

                    break;

                default:
                    return;
            }
        }

        private void DecryptEmployeeData(DynamicReportModel data)
        {
            data.Salary = cryptoService.Decrypt(data.Salary);
            data.EbankingAmount = cryptoService.Decrypt(data.EbankingAmount);
            data.DirectBanking1Amount = cryptoService.Decrypt(data.DirectBanking1Amount);
            data.DirectBanking2Amount = cryptoService.Decrypt(data.DirectBanking2Amount);
            data.CashAmount = cryptoService.Decrypt(data.CashAmount);

            data.AllowanceDirectBanking1Amount = cryptoService.Decrypt(data.DirectBanking1Amount);
            data.AllowanceDirectBanking2Amount = cryptoService.Decrypt(data.DirectBanking2Amount);
            data.AllowanceCashAmount = cryptoService.Decrypt(data.CashAmount);
            data.SubsistenceAmount = cryptoService.Decrypt(data.SubsistenceAmount);
            data.HomePassageAmount = cryptoService.Decrypt(data.HomePassageAmount);
            data.HousingAmount = cryptoService.Decrypt(data.HousingAmount);
            data.MobilePhoneAmount = cryptoService.Decrypt(data.MobilePhoneAmount);
            data.HPIAmount = cryptoService.Decrypt(data.HPIAmount);
            data.DutyMealAmount = cryptoService.Decrypt(data.DutyMealAmount);
            data.SecondmentAmount = cryptoService.Decrypt(data.SecondmentAmount);
            data.OthersAmount = cryptoService.Decrypt(data.OthersAmount);

            data.RoomingChargesAmount = cryptoService.Decrypt(data.RoomingChargesAmount);
            data.FitnessFirstAmount = cryptoService.Decrypt(data.FitnessFirstAmount);
        }
    }
}