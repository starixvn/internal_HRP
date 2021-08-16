namespace HRP.Repositories
{
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
        public async Task<(PaymentProcess, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>, IEnumerable<ValidationResult>)> GetUploadingFilesResultAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var paymentProcess = await multi.ReadFirstOrDefaultAsync<PaymentProcess>();
                    var attendanceErrors = await multi.ReadAsync<ValidationResult>();
                    var dstNetSalaryErrors = await multi.ReadAsync<ValidationResult>();
                    var additionDeductionErrors = await multi.ReadAsync<ValidationResult>();

                    return (paymentProcess, attendanceErrors, dstNetSalaryErrors, additionDeductionErrors);
                }
            }
        }

        public async Task<GeneratePayrollModel> GetDataForGeneratePayrollAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var monthlyPayroll = await multi.ReadAsync<MonthlyPayroll>();
                    var monthlyAttendance = await multi.ReadAsync<MonthlyAttendance>();
                    var monthlyDSTNetSalary = await multi.ReadAsync<MonthlyDSTNetSalary>();
                    var additionDeductions = await multi.ReadAsync<AdditionDeductionUploading>();
                    var monthlyBasicSalary = await multi.ReadAsync<MonthlyBasicSalaryModel>();
                    var monthlyBasicAllowance = await multi.ReadAsync<MonthlyPayrollBasicAllowanceModel>();
                    var currencyExchangeRate = await multi.ReadAsync<CurrencyExchangeRateModel>();

                    var result = new GeneratePayrollModel
                    {
                        MonthlyPayroll = monthlyPayroll,
                        MonthlyAttendance = monthlyAttendance,
                        MonthlyDSTNetSalary = monthlyDSTNetSalary,
                        MonthlyBasicSalary = monthlyBasicSalary,
                        MonthlyBasicAllowance = monthlyBasicAllowance,
                        CurrencyExchangeRate = currencyExchangeRate,
                        AdditionDeductions = additionDeductions
                    };

                    return result;
                }
            }
        }
    }
}