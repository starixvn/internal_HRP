namespace HRP.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.Employee;

    public class EmployeeRepository : IEmployeeRepository
    {
        public async Task<Employment> GetEmploymentByEmployeeIdAsync(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria).ConfigureAwait(false))
                {
                    var employment = await multi.ReadFirstOrDefaultAsync<Employment>().ConfigureAwait(false);

                    if (employment != null)
                    {
                        employment.WorkPermit = await multi.ReadFirstOrDefaultAsync<WorkPermit>().ConfigureAwait(false);
                        employment.Paymaster = await multi.ReadFirstOrDefaultAsync<Paymaster>().ConfigureAwait(false);
                        employment.EPA = await multi.ReadFirstOrDefaultAsync<EPA>().ConfigureAwait(false);

                        return employment;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }
    }
}