namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.Payslip;

    public class PayslipRepository : IPayslipRepository
    {
        public async Task<PayslipDetailModel> GetPayslipDetailAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var payslip = await multi.ReadFirstOrDefaultAsync<PayslipDetailModel>().ConfigureAwait(false);

                    if (payslip != null)
                    {
                        payslip.SalaryAdditions = await multi.ReadAsync<MonthlySalaryAdditionModel>().ConfigureAwait(false);
                        payslip.SalaryDeductions = await multi.ReadAsync<MonthlySalaryDeductionModel>().ConfigureAwait(false);
                        payslip.AllowanceAdditions = await multi.ReadAsync<MonthlyAllowanceAdditionModel>().ConfigureAwait(false);
                        payslip.AllowanceDeductions = await multi.ReadAsync<MonthlyAllowanceDeductionModel>().ConfigureAwait(false);
                    }

                    return payslip;
                }
            }
        }
    }
}