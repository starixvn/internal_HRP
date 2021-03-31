namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Models.Report;

    public interface IReportRepository
    {
        Task<DirectBankingReport> GetDataForBankInReportAsync(ICriteria criteria);

        Task<MonthlyPayrollReport> GetDataForMonthlyPayrollReportAsync(ICriteria criteria);
    }
}