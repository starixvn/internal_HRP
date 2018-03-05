namespace HRP.Repositories
{
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models;

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
    }
}