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
                using (IMultiResultReader result = await db.QueryMultipleAsync(criteria))
                {
                    return new MonthlyPayrollReport
                    {
                        Employees = await result.ReadAsync<EmployeeBasicInfo>(),
                        MonthlyBasicSalary = await result.ReadAsync<MonthlyBasicSalaryModel>(),
                        MonthlySalary = await result.ReadAsync<MonthlySalaryModel>(),
                        MonthlyBasicAllowance = await result.ReadAsync<MonthlyPayrollBasicAllowanceModel>(),
                        MonthlyAllowance = await result.ReadAsync<MonthlyAllowanceModel>(),
                        CurrencyExchangeRate = await result.ReadAsync<CurrencyExchangeRateModel>(),
                        CurrencyList = await result.ReadAsync<Currency>(),
                        Additions = await result.ReadAsync<MonthlySalaryAdditionModel>(),
                        Deductions = await result.ReadAsync<MonthlySalaryDeductionModel>()
                    };
                }
            }
        }
    }
}