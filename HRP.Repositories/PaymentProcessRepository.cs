namespace HRP.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.PaymentProcess;
    using HRP.Models.ServiceModels;

    public class PaymentProcessRepository : IPaymentProcessRepository
    {
        public async Task<Tuple<PaymentProcess, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>>> GetUploadingFilesResultAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var paymentProcess = await multi.ReadFirstOrDefaultAsync<PaymentProcess>().ConfigureAwait(false);
                    var attendanceErrors = await multi.ReadAsync<ValidationResult>().ConfigureAwait(false);
                    var dstNetSalaryErrors = await multi.ReadAsync<ValidationResult>().ConfigureAwait(false);

                    return new Tuple<PaymentProcess, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>>(paymentProcess, attendanceErrors, dstNetSalaryErrors);
                }
            }
        }

        public async Task<GeneratePayrollModel> GetDataForGeneratePayroll(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var monthlyPayroll = await multi.ReadAsync<MonthlyPayroll>().ConfigureAwait(false);
                    var monthlyAttendance = await multi.ReadAsync<MonthlyAttendance>().ConfigureAwait(false);
                    var monthlyDSTNetSalary = await multi.ReadAsync<MonthlyDSTNetSalary>().ConfigureAwait(false);
                    var monthlyBasicSalary = await multi.ReadAsync<MonthlyBasicSalaryModel>().ConfigureAwait(false);
                    var monthlyBasicAllowance = await multi.ReadAsync<MonthlyPayrollBasicAllowanceModel>().ConfigureAwait(false);
                    var currencyExchangeRate = await multi.ReadAsync<CurrencyExchangeRateModel>().ConfigureAwait(false);

                    var result = new GeneratePayrollModel
                    {
                        MonthlyPayroll = monthlyPayroll,
                        MonthlyAttendance = monthlyAttendance,
                        MonthlyDSTNetSalary = monthlyDSTNetSalary,
                        MonthlyBasicSalary = monthlyBasicSalary,
                        MonthlyBasicAllowance = monthlyBasicAllowance,
                        CurrencyExchangeRate = currencyExchangeRate
                    };

                    return result;
                }
            }
        }
    }
}