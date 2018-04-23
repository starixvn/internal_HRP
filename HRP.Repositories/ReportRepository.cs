namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Models.Report;

    public class ReportRepository : IReportRepository
    {
        public async Task<BankInReport> GetDataForBankInReportAsync(ICriteria criteria)
        {
            using (var database = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader results = await database.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var bankInReport = new BankInReport
                    {
                        EBankingBankIns = await results.ReadAsync<BankIn>().ConfigureAwait(false),
                        CashBankIns = await results.ReadAsync<BankIn>().ConfigureAwait(false),
                        CompanyName = await results.ReadSingleAsync<string>().ConfigureAwait(false),
                        SelectedPaymentTransactions = await results.ReadAsync<string>().ConfigureAwait(false)
                    };

                    return bankInReport;
                }
            }
        }
    }
}
