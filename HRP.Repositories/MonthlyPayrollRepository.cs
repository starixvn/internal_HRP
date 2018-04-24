namespace HRP.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.MonthlyPayroll;

    public class MonthlyPayrollRepository : IMonthlyPayrollRepository
    {
        public async Task<Tuple<MonthlyPayrollFilterInfoModel,
            MonthlyBasicSalaryModel,
            MonthlySalaryModel,
            IEnumerable<MonthlySalaryAdditionModel>,
            IEnumerable<MonthlySalaryDeductionModel>>> GetMonthlySalaryAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var filterInfo = await multi.ReadFirstOrDefaultAsync<MonthlyPayrollFilterInfoModel>().ConfigureAwait(false);
                    var monthlyBasicSalary = await multi.ReadFirstAsync<MonthlyBasicSalaryModel>().ConfigureAwait(false);
                    var monthlySalary = await multi.ReadFirstAsync<MonthlySalaryModel>().ConfigureAwait(false);
                    var additions = await multi.ReadAsync<MonthlySalaryAdditionModel>().ConfigureAwait(false);
                    var deductions = await multi.ReadAsync<MonthlySalaryDeductionModel>().ConfigureAwait(false);

                    var tuple = new Tuple<MonthlyPayrollFilterInfoModel,
                        MonthlyBasicSalaryModel,
                        MonthlySalaryModel,
                        IEnumerable<MonthlySalaryAdditionModel>,
                        IEnumerable<MonthlySalaryDeductionModel>>(filterInfo, monthlyBasicSalary, monthlySalary, additions, deductions);

                    return tuple;
                }
            }
        }

        public async Task<Tuple<MonthlyAllowanceModel,
            IEnumerable<MonthlyAllowanceAdditionModel>,
            IEnumerable<MonthlyAllowanceDeductionModel>,
            MonthlyPayrollBasicAllowanceModel,
            MonthlyPayrollFilterInfoModel>> GetMonthlyAllowancesAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (var multiResults = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var allowance = multiResults.ReadFirstOrDefault<MonthlyAllowanceModel>();
                    var additions = multiResults.Read<MonthlyAllowanceAdditionModel>();
                    var deductions = multiResults.Read<MonthlyAllowanceDeductionModel>();
                    var basicAllowance = multiResults.ReadFirstOrDefault<MonthlyPayrollBasicAllowanceModel>();
                    var info = multiResults.ReadFirstOrDefault<MonthlyPayrollFilterInfoModel>();

                    var tuple = new Tuple<MonthlyAllowanceModel,
                        IEnumerable<MonthlyAllowanceAdditionModel>,
                        IEnumerable<MonthlyAllowanceDeductionModel>,
                        MonthlyPayrollBasicAllowanceModel,
                        MonthlyPayrollFilterInfoModel>(allowance, additions, deductions, basicAllowance, info);

                    return tuple;
                }
            }
        }
    }
}