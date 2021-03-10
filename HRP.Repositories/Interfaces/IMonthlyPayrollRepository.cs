namespace HRP.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Models.MonthlyPayroll;
    using HRP.Models.ServiceModels;

    public interface IMonthlyPayrollRepository
    {
        Task<(MonthlyBasicSalaryModel,
            MonthlySalaryModel,
            IEnumerable<MonthlySalaryAdditionModel>,
            IEnumerable<MonthlySalaryDeductionModel>)> GetMonthlySalaryAsync(ICriteria criteria);

        Task<(MonthlyAllowanceModel,
            IEnumerable<MonthlyAllowanceAdditionModel>,
            IEnumerable<MonthlyAllowanceDeductionModel>,
            MonthlyPayrollBasicAllowanceModel)> GetMonthlyAllowancesAsync(ICriteria criteria);
    }
}