namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Core.Model;
    using HRP.Models.ServiceModels;
    using Models.Currency;
    using Models.Employee;
    using Models.MonthlyPayroll;
    using Models.Report;

    public class ReportRepository : IReportRepository
    {
        public async Task<DirectBankingReport> GetDataForBankInReportAsync(ICriteria criteria)
        {
            using (var database = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader results = await database.QueryMultipleAsync(criteria))
                {
                    DirectBankingReport report = null;
                    if (!results.IsEmptyOrClosed)
                    {
                        report = new DirectBankingReport
                        {
                            DirectBankingBankIns = await results.ReadAsync<BankIn>(),
                            CashBankIns = await results.ReadAsync<BankIn>(),
                            CompanyName = await results.ReadSingleAsync<string>(),
                            PaymentTransactions = await results.ReadAsync<ReferenceResourceModel>()
                        };
                    }

                    return report;
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