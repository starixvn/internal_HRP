namespace HRP.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.ServiceModels;

    public interface IMonthlyPayrollRepository
    {
        Task<Tuple<MonthlyBasicSalaryModel,
            MonthlySalaryModel,
            IEnumerable<MonthlySalaryAdditionModel>,
            IEnumerable<MonthlySalaryDeductionModel>>> GetMonthlySalaryAsync(ICriteria criteria);

        Task<Tuple<MonthlyAllowance,
            IEnumerable<MonthlyAllowanceAddition>,
            IEnumerable<MonthlyAllowanceDeduction>,
            MonthlyPayrollBasicAllowance>> GetMonthlyAllowancesAsync(ICriteria criteria);
    }
}