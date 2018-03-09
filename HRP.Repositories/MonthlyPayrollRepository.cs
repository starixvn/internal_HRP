namespace HRP.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models;
    using HRP.Models.MonthlyPayroll;

    public class MonthlyPayrollRepository : IMonthlyPayrollRepository
    {
        public async Task<EmployeeMonthlyPayrollViewModel> GetDataFilterForIndexPageAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var viewModel = await multi.ReadFirstAsync<EmployeeMonthlyPayrollViewModel>().ConfigureAwait(false);
                    var divisions = await multi.ReadAsync<MultiSelectViewModel>().ConfigureAwait(false);
                    var departments = await multi.ReadAsync<MultiSelectViewModel>().ConfigureAwait(false);
                    var costCenters = await multi.ReadAsync<MultiSelectViewModel>().ConfigureAwait(false);

                    viewModel.HierarchyCompanies = divisions.Select(division => new MultiSelectViewModel
                    {
                        Value = division.Value,
                        Text = division.Text,
                        Checked = division.Checked,
                        Items = departments.Where(dept => dept.ParentId == division.Value).Select(dept => new MultiSelectViewModel
                        {
                            Value = dept.Value,
                            Text = dept.Text,
                            Checked = dept.Checked,
                            Items = costCenters.Where(costcenter => costcenter.ParentId == dept.Value).Select(costcenter => new MultiSelectViewModel
                            {
                                Value = costcenter.Value,
                                Text = costcenter.Text,
                                Checked = costcenter.Checked
                            })
                        })
                    });

                    return viewModel;
                }
            }
        }

        public async Task<IEnumerable<MonthlyPayrollEmployeeInfoViewModel>> GetMonthlyPayrollAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var monhtlyPayrolls = await multi.ReadAsync<MonthlyPayrollEmployeeInfoViewModel>().ConfigureAwait(false);
                    var basicSalaries = await multi.ReadAsync<MonthlyPayrollBasicSalaryViewModel>().ConfigureAwait(false);
                    var allowances = await multi.ReadAsync<MonthlyPayrollBasicSalaryViewModel>().ConfigureAwait(false);

                    foreach (var monhtlyPayroll in monhtlyPayrolls)
                    {
                        monhtlyPayroll.BasicSalary = basicSalaries.First(x => x.PayrollId == monhtlyPayroll.PayrollId);
                        monhtlyPayroll.Allowance = allowances.First(x => x.PayrollId == monhtlyPayroll.PayrollId);
                    }

                    return monhtlyPayrolls;
                }
            }
        }

        public async Task<EditMonthlySalaryViewModel> GetMonthlySalaryToEditAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var viewModel = await multi.ReadFirstOrDefaultAsync<EditMonthlySalaryViewModel>().ConfigureAwait(false);

                    if (viewModel != null)
                    {
                        viewModel.MonthlyBasicSalary = await multi.ReadFirstAsync<MonthlyBasicSalaryViewModel>().ConfigureAwait(false);
                        viewModel.MonthlySalary = await multi.ReadFirstAsync<MonthlySalaryViewModel>().ConfigureAwait(false);
                        viewModel.Additions = await multi.ReadAsync<MonthlySalaryAdditionViewModel>().ConfigureAwait(false);
                        viewModel.Deductions = await multi.ReadAsync<MonthlySalaryDeductionViewModel>().ConfigureAwait(false);
                    }

                    return viewModel;
                }
            }
        }
    }
}