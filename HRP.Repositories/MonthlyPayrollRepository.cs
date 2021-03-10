namespace HRP.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.ServiceModels;

    public class MonthlyPayrollRepository : IMonthlyPayrollRepository
    {
        public async Task<(MonthlyBasicSalaryModel,
            MonthlySalaryModel,
            IEnumerable<MonthlySalaryAdditionModel>,
            IEnumerable<MonthlySalaryDeductionModel>)> GetMonthlySalaryAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var monthlyBasicSalary = await multi.ReadFirstOrDefaultAsync<MonthlyBasicSalaryModel>();
                    var monthlySalary = await multi.ReadFirstOrDefaultAsync<MonthlySalaryModel>();
                    var additions = await multi.ReadAsync<MonthlySalaryAdditionModel>();
                    var deductions = await multi.ReadAsync<MonthlySalaryDeductionModel>();

                    return (monthlyBasicSalary, monthlySalary ?? new MonthlySalaryModel(), additions, deductions);
                }
            }
        }

        public async Task<(MonthlyAllowanceModel,
            IEnumerable<MonthlyAllowanceAdditionModel>,
            IEnumerable<MonthlyAllowanceDeductionModel>,
            MonthlyPayrollBasicAllowanceModel)> GetMonthlyAllowancesAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multiResults = await db.QueryMultipleAsync(criteria))
                {
                    var allowance = multiResults.ReadFirstOrDefault<MonthlyAllowanceModel>();
                    var additions = multiResults.Read<MonthlyAllowanceAdditionModel>();
                    var deductions = multiResults.Read<MonthlyAllowanceDeductionModel>();
                    var basicAllowance = multiResults.ReadFirstOrDefault<MonthlyPayrollBasicAllowanceModel>();

                    return (allowance, additions, deductions, basicAllowance);
                }
            }
        }
    }
}