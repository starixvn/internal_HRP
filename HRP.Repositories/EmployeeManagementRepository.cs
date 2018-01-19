namespace HRP.Repositories.Interfaces
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Core.Constants;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;

    public class EmployeeManagementRepository : IEmployeeManagementRepository
    {
        public CreateEmploymentViewModel GetDataToCreateEmployment(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var companies = multi.Read<Company>();
                    var jobCategories = multi.Read<JobCategory>();
                    var refResources = multi.Read<ReferenceResourceModel>();
                    var model = new CreateEmploymentViewModel
                    {
                        Companies = companies,
                        LocalCompanies = GetRefResourceByName(refResources, ReferenceResource.LocalCompany),
                        LocalJobTitles = GetRefResourceByName(refResources, ReferenceResource.LocalJobTitle),
                        EmploymentTypes = GetRefResourceByName(refResources, ReferenceResource.EmploymentType),
                        EmploymentStatuses = GetRefResourceByName(refResources, ReferenceResource.EmploymentStatus),
                        JobCategories = jobCategories,
                        Countries = GetRefResourceByName(refResources, ReferenceResource.Country),
                        Cities = GetRefResourceByName(refResources, ReferenceResource.City),
                        Offices = GetRefResourceByName(refResources, ReferenceResource.Office),
                        WorkPermitType1s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType1),
                        WorkPermitType2s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType2),
                        CommonReasons = GetRefResourceByName(refResources, ReferenceResource.CommonReasons),
                        GroupDateReasons = GetRefResourceByName(refResources, ReferenceResource.GroupDateReasons)
                    };

                    return model;
                }
            }
        }

        public UpdateEmploymentViewModel GetDataToUpdateEmployment(ICriteria criteria)
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

                        employment.Companies = multi.Read<Company>();
                        employment.Divisions = multi.Read<Division>();
                        employment.Departments = multi.Read<Department>();
                        employment.Sections = multi.Read<Section>();
                        employment.CostCenters = multi.Read<CostCenter>();
                        employment.JobCategories = multi.Read<JobCategory>();
                        employment.GradeLevels = multi.Read<GradeLevel>();
                        employment.JobGrades = multi.Read<JobGrade>();

                        var refResources = multi.Read<ReferenceResourceModel>();
                        employment.LocalCompanies = GetRefResourceByName(refResources, ReferenceResource.LocalCompany);
                        employment.LocalJobTitles = GetRefResourceByName(refResources, ReferenceResource.LocalJobTitle);
                        employment.EmploymentTypes = GetRefResourceByName(refResources, ReferenceResource.EmploymentType);
                        employment.EmploymentStatuses = GetRefResourceByName(refResources, ReferenceResource.EmploymentStatus);
                        employment.Countries = GetRefResourceByName(refResources, ReferenceResource.Country);
                        employment.Cities = GetRefResourceByName(refResources, ReferenceResource.City);
                        employment.Offices = GetRefResourceByName(refResources, ReferenceResource.Office);
                        employment.WorkPermitType1s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType1);
                        employment.WorkPermitType2s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType2);
                        employment.CommonReasons = GetRefResourceByName(refResources, ReferenceResource.CommonReasons);
                        employment.GroupDateReasons = GetRefResourceByName(refResources, ReferenceResource.GroupDateReasons);
                    }

                    return employment;
                }
            }
        }

        private static IEnumerable<ReferenceResourceModel> GetRefResourceByName(IEnumerable<ReferenceResourceModel> refResources, string refName)
        {
            return refResources.Where(x => x.RefName == refName);
        }
    }
}