namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.PaymentProcess;
    using HRP.Repositories.Interfaces;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class PaymentProcessRepository : IPaymentProcessRepository
    {
        public async Task<UploadAttendanceAndSalaryViewModel> GetUploadingFilesResultAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var paymentProcess = await multi.ReadFirstOrDefaultAsync<PaymentProcess>().ConfigureAwait(false);
                    var attendanceErrors = await multi.ReadAsync<ValidationResult>().ConfigureAwait(false);
                    var dstNetSalaryErrors = await multi.ReadAsync<ValidationResult>().ConfigureAwait(false);

                    var result = new UploadAttendanceAndSalaryViewModel();

                    if (paymentProcess != null)
                    {
                        result.CompanyId = paymentProcess.CompanyId;
                        result.SelectedTime = paymentProcess.Month;
                        result.CompanyCode = paymentProcess.CompanyCode;
                        result.StartDate = paymentProcess.StartDate;
                        result.DueDate = paymentProcess.DueDate;

                        if (paymentProcess.AttendanceFileName != null)
                        {
                            result.AttendanceFileName = paymentProcess.AttendanceFileName;
                            result.AttendanceResult = new FileResultViewModel
                            {
                                TotalRow = paymentProcess.AttendanceTotalRow,
                                ValidationErrors = attendanceErrors
                            };
                        }

                        if (paymentProcess.DSTNetSalaryFileName != null)
                        {
                            result.DSTNetSalaryFileName = paymentProcess.DSTNetSalaryFileName;
                            result.DSTNetSalaryResult = new FileResultViewModel
                            {
                                TotalRow = paymentProcess.DSTNetSalaryTotalRow,
                                ValidationErrors = dstNetSalaryErrors
                            };
                        }
                    }

                    return result;
                }
            }
        }

        public async Task<GeneratePayrollViewModel> GetDataForGeneratePayroll(ICriteria criteria)
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

                    var result = new GeneratePayrollViewModel
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

        public async Task<SelectWorkingMonthCompanyViewModel> GetDataForSelectWorkingMonthCompanyAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var companies = await multi.ReadAsync<Company>().ConfigureAwait(false);
                    var paymentProcess = await multi.ReadFirstOrDefaultAsync<PaymentProcess>().ConfigureAwait(false);

                    var result = new SelectWorkingMonthCompanyViewModel
                    {
                        Companies = new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code)),
                        SelectedTime = paymentProcess.Month,
                        StartDate = paymentProcess.StartDate,
                        DueDate = paymentProcess.DueDate
                    };

                    return result;
                }
            }
        }
    }
}