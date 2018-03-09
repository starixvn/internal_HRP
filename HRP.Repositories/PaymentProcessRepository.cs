namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Core.Model;
    using HRP.Models.PaymentProcess;
    using HRP.Repositories.Interfaces;

    public class PaymentProcessRepository : IPaymentProcessRepository
    {
        public async Task<ValidationFilesResultViewModel> GetValidationFilesResultAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var paymentProcess = await multi.ReadFirstOrDefaultAsync<PaymentProcess>().ConfigureAwait(false);
                    var attendanceErrors = await multi.ReadAsync<ValidationResult>().ConfigureAwait(false);
                    var dstNetSalaryErrors = await multi.ReadAsync<ValidationResult>().ConfigureAwait(false);

                    var result = new ValidationFilesResultViewModel();

                    if (paymentProcess != null)
                    {
                        if (paymentProcess.AttendanceFileName != null)
                        {
                            result.AttendanceFile = new FileResultViewModel
                            {
                                TotalRow = paymentProcess.AttendanceTotalRow,
                                ValidationErrors = attendanceErrors
                            };
                        }

                        if (paymentProcess.DSTNetSalaryFileName != null)
                        {
                            result.DSTNetSalaryFile = new FileResultViewModel
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