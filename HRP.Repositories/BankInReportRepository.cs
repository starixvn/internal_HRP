namespace HRP.Repositories
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Interfaces;
    using Models.Report;

    public class ReportRepository : IReportRepository
    {
        public async Task<BankInReport> GetDataForBankInReportAsync(ICriteria criteria)
        {
            using (var database = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader results = database.QueryMultiple(criteria))
                {
                    var bankInReport = new BankInReport
                    {
                        EBankingBankIns = await results.ReadAsync<BankIn>().ConfigureAwait(false),
                        DirectBanking1BankIns = await results.ReadAsync<BankIn>().ConfigureAwait(false),
                        DirectBanking2BankIns = await results.ReadAsync<BankIn>().ConfigureAwait(false),
                        CashBankIns = await results.ReadAsync<BankIn>().ConfigureAwait(false),
                        CompanyName = await results.ReadSingleAsync<string>().ConfigureAwait(false)
                    };

                    return bankInReport;
                }
            }
        }
    }
}
