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
    using Models.Currency;
    using Models.MonthlyPayroll;
    using Models.Report;
    using Repositories;

    public class ReportService : IReportService
    {
        private readonly ICryptoService cryptoService;
        private readonly IReportRepository reportRepository;
        private readonly ISessionService sessionService;

        public ReportService(
            ICryptoService cryptoService,
            ISessionService sessionService,
            IReportRepository reportRepository = null)
        {
            this.cryptoService = cryptoService;
            this.sessionService = sessionService;
            this.reportRepository = reportRepository ?? new ReportRepository();
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

            report.CompanyCode = report.Employees?.FirstOrDefault()?.CompanyCode;
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

        private static decimal GetExchangeRate(
            IEnumerable<CurrencyExchangeRateModel> currencyExchangeRates,
            int fromCurrencyId,
            int toCurrencyId)
        {
            CurrencyExchangeRateModel currencyExchangeRate =
                currencyExchangeRates.FirstOrDefault(x => x.FromCurrencyId == fromCurrencyId && x.ToCurrencyId == toCurrencyId);

            return currencyExchangeRate?.ExchangeRate ?? 0;
        }

        #endregion MonthlyPayrollReport
    }
}