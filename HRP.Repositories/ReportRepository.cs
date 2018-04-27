namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.Currency;
    using HRP.Models.Employee;
    using HRP.Models.MonthlyPayroll;
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

        public async Task<MonthlyPayrollReport> GetDataForMonthlyPayrollReportAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader results = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    return new MonthlyPayrollReport
                    {
                        Employees = await results.ReadAsync<EmployeeBasicInfo>().ConfigureAwait(false),
                        MonthlyBasicSalary = await results.ReadAsync<MonthlyBasicSalaryModel>().ConfigureAwait(false),
                        MonthlySalary = await results.ReadAsync<MonthlySalaryModel>().ConfigureAwait(false),
                        MonthlyBasicAllowance = await results.ReadAsync<MonthlyPayrollBasicAllowanceModel>().ConfigureAwait(false),
                        MonthlyAllowance = await results.ReadAsync<MonthlyAllowanceModel>().ConfigureAwait(false),
                        CurrencyExchangeRate = await results.ReadAsync<CurrencyExchangeRateModel>().ConfigureAwait(false)
                    };
                }
            }
        }
    }
}