namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.ServiceModels;
    using Models.Currency;
    using Models.Employee;
    using Models.MonthlyPayroll;
    using Models.Report;

    public class ReportRepository : IReportRepository
    {
        public async Task<BankInReport> GetDataForBankInReportAsync(ICriteria criteria)
        {
            using (var database = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader results = await database.QueryMultipleAsync(criteria))
                {
                    BankInReport bankInReport = null;

                    if (!results.IsEmptyOrClosed)
                    {
                        bankInReport = new BankInReport
                        {
                            EBankingBankIns = await results.ReadAsync<BankIn>(),
                            CashBankIns = await results.ReadAsync<BankIn>(),
                            CompanyName = await results.ReadSingleAsync<string>(),
                            SelectedPaymentTransactions = await results.ReadAsync<string>()
                        };
                    }

                    return bankInReport;
                }
            }
        }

        public async Task<MonthlyPayrollReport> GetDataForMonthlyPayrollReportAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader results = await db.QueryMultipleAsync(criteria))
                {
                    return new MonthlyPayrollReport
                    {
                        Employees = await results.ReadAsync<EmployeeBasicInfo>(),
                        MonthlyBasicSalary = await results.ReadAsync<MonthlyBasicSalaryModel>(),
                        MonthlySalary = await results.ReadAsync<MonthlySalaryModel>(),
                        MonthlyBasicAllowance = await results.ReadAsync<MonthlyPayrollBasicAllowanceModel>(),
                        MonthlyAllowance = await results.ReadAsync<MonthlyAllowanceModel>(),
                        CurrencyExchangeRate = await results.ReadAsync<CurrencyExchangeRateModel>(),
                        CurrencyList = await results.ReadAsync<Currency>()
                    };
                }
            }
        }
    }
}