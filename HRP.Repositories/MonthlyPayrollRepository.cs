namespace HRP.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.ServiceModels;

    public class MonthlyPayrollRepository : IMonthlyPayrollRepository
    {
        public async Task<Tuple<MonthlyBasicSalaryModel,
            MonthlySalaryModel,
            IEnumerable<MonthlySalaryAdditionModel>,
            IEnumerable<MonthlySalaryDeductionModel>>> GetMonthlySalaryAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var monthlyBasicSalary = await multi.ReadFirstOrDefaultAsync<MonthlyBasicSalaryModel>().ConfigureAwait(false);
                    var monthlySalary = await multi.ReadFirstOrDefaultAsync<MonthlySalaryModel>().ConfigureAwait(false);
                    var additions = await multi.ReadAsync<MonthlySalaryAdditionModel>().ConfigureAwait(false);
                    var deductions = await multi.ReadAsync<MonthlySalaryDeductionModel>().ConfigureAwait(false);

                    var tuple = new Tuple<MonthlyBasicSalaryModel,
                        MonthlySalaryModel,
                        IEnumerable<MonthlySalaryAdditionModel>,
                        IEnumerable<MonthlySalaryDeductionModel>>(monthlyBasicSalary, monthlySalary ?? new MonthlySalaryModel(), additions, deductions);

                    return tuple;
                }
            }
        }

        public async Task<Tuple<MonthlyAllowanceModel,
            IEnumerable<MonthlyAllowanceAdditionModel>,
            IEnumerable<MonthlyAllowanceDeductionModel>,
            MonthlyPayrollBasicAllowanceModel>> GetMonthlyAllowancesAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multiResults = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var allowance = multiResults.ReadFirstOrDefault<MonthlyAllowanceModel>();
                    var additions = multiResults.Read<MonthlyAllowanceAdditionModel>();
                    var deductions = multiResults.Read<MonthlyAllowanceDeductionModel>();
                    var basicAllowance = multiResults.ReadFirstOrDefault<MonthlyPayrollBasicAllowanceModel>();

                    var tuple = new Tuple<MonthlyAllowanceModel,
                        IEnumerable<MonthlyAllowanceAdditionModel>,
                        IEnumerable<MonthlyAllowanceDeductionModel>,
                        MonthlyPayrollBasicAllowanceModel>(allowance, additions, deductions, basicAllowance);

                    return tuple;
                }
            }
        }
    }
}