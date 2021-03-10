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
                using (IMultiResultReader multi = await db.QueryMultipleAsync(criteria))
                {
                    var employment = await multi.ReadFirstOrDefaultAsync<Employment>();

                    if (employment != null)
                    {
                        employment.WorkPermit = await multi.ReadFirstOrDefaultAsync<WorkPermit>();
                        employment.Paymaster = await multi.ReadFirstOrDefaultAsync<Paymaster>();
                        employment.EPA = await multi.ReadFirstOrDefaultAsync<EPA>();
                    }

                    return employment;
                }
            }
        }
    }
}