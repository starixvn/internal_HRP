namespace HRP.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.Payslip;

    public class PayslipRepository : IPayslipRepository
    {
        public async Task<IEnumerable<PayslipDetailModel>> GetPayslipDetailsAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var payslips = await multi.ReadAsync<PayslipDetailModel>().ConfigureAwait(false);
                    var salaryAdditions = await multi.ReadAsync<MonthlySalaryAdditionModel>().ConfigureAwait(false);
                    var salaryDeductions = await multi.ReadAsync<MonthlySalaryDeductionModel>().ConfigureAwait(false);
                    var allowanceAdditions = await multi.ReadAsync<MonthlyAllowanceAdditionModel>().ConfigureAwait(false);
                    var allowanceDeductions = await multi.ReadAsync<MonthlyAllowanceDeductionModel>().ConfigureAwait(false);

                    foreach (PayslipDetailModel payslip in payslips)
                    {
                        payslip.SalaryAdditions = salaryAdditions.Where(x => x.PayrollId == payslip.PayrollId).ToList();
                        payslip.SalaryDeductions = salaryDeductions.Where(x => x.PayrollId == payslip.PayrollId).ToList();
                        payslip.AllowanceAdditions = allowanceAdditions.Where(x => x.PayrollId == payslip.PayrollId).ToList();
                        payslip.AllowanceDeductions = allowanceDeductions.Where(x => x.PayrollId == payslip.PayrollId).ToList();
                    }

                    return payslips;
                }
            }
        }
    }
}