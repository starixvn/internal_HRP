namespace HRP.Repositories.Interfaces
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Models.EmployeeManagement;

    public class EmployeeManagementRepository : IEmployeeManagementRepository
    {
        public UpdateEmploymentViewModel GetEmploymentToUpdate(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var employment = multi.ReadFirstOrDefault<UpdateEmploymentViewModel>();

                    if (employment != null)
                    {
                        employment.WorkPermit = multi.ReadFirstOrDefault<WorkPermit>();
                        employment.Paymaster = multi.ReadFirstOrDefault<Paymaster>();
                        employment.EPA = multi.ReadFirstOrDefault<EPA>();
                    }

                    return employment ?? new UpdateEmploymentViewModel();
                }
            }
        }
    }
}