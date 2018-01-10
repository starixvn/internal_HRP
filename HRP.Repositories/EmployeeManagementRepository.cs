namespace HRP.Repositories.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.EmployeeManagement;

    public class EmployeeManagementRepository : IEmployeeManagementRepository
    {
        public async Task<UpdateEmploymentViewModel> GetEmploymentToUpdateAsync(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var employment = await multi.ReadSingleAsync<UpdateEmploymentViewModel>().ConfigureAwait(false);
                    employment.WorkPermit = await multi.ReadSingleAsync<WorkPermit>().ConfigureAwait(false);
                    employment.Paymaster = await multi.ReadSingleAsync<Paymaster>().ConfigureAwait(false);
                    employment.EPA = await multi.ReadSingleAsync<EPA>().ConfigureAwait(false);

                    return employment;
                }
            }
        }
    }
}