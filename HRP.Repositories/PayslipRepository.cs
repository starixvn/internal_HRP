namespace HRP.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.Payslip;
    using HRP.Models.ServiceModels;

    public class PayslipRepository : IPayslipRepository
    {
        public async Task<IEnumerable<PayslipDetailModel>> GetPayslipDetailsAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var payslips = await multi.ReadAsync<PayslipDetailModel>();
                    var salaryAdditions = await multi.ReadAsync<MonthlySalaryAdditionModel>();
                    var salaryDeductions = await multi.ReadAsync<MonthlySalaryDeductionModel>();
                    var allowanceAdditions = await multi.ReadAsync<MonthlyAllowanceAdditionModel>();
                    var allowanceDeductions = await multi.ReadAsync<MonthlyAllowanceDeductionModel>();

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