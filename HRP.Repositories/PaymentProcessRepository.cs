namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.PaymentProcess;
    using HRP.Repositories.Interfaces;

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
                        result.CompanyCode = paymentProcess.CompanyCode;

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
    }
}