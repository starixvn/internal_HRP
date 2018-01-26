namespace HRP.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using HRP.Core.Constants;
    using HRP.Models;
    using HRP.Models.EmployeeManagement;
    using HRP.Repositories.Interfaces;

    public class EmployeeManagementRepository : IEmployeeManagementRepository
    {
        public CreateEmploymentViewModel GetDataToCreateEmployment(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var companies = multi.Read<Company>();
                    var jobCategories = multi.Read<JobCategory>();
                    var coutries = multi.Read<Country>();
                    var refResources = multi.Read<ReferenceResourceModel>();
                    var model = new CreateEmploymentViewModel
                    {
                        Companies = companies,
                        LocalCompanies = GetRefResourceByName(refResources, ReferenceResource.LocalCompany),
                        LocalJobTitles = GetRefResourceByName(refResources, ReferenceResource.LocalJobTitle),
                        EmploymentTypes = GetRefResourceByName(refResources, ReferenceResource.EmploymentType),
                        EmploymentStatuses = GetRefResourceByName(refResources, ReferenceResource.EmploymentStatus),
                        JobCategories = jobCategories,
                        Countries = coutries,
                        WorkPermitType1s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType1),
                        WorkPermitType2s = GetRefResourceByName(refResources, ReferenceResource.WorkPermitType2)
                    };

                    return model;
                }
            }
        }

        public UpdateEmploymentViewModel GetDataToUpdateEmployment(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
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
                        employment.CostCenters = multi.Read<CostCenter>();
                        employment.Sections = multi.Read<Section>();
                        employment.JobCategories = multi.Read<JobCategory>();
                        employment.GradeLevels = multi.Read<GradeLevel>();
                        employment.JobGrades = multi.Read<JobGrade>();
                        employment.Countries = multi.Read<Country>();
                        employment.Cities = multi.Read<City>();
                        employment.Offices = multi.Read<Office>();

                        var refResources = multi.Read<ReferenceResourceModel>();
                        employment.LocalCompanies = GetRefResourceByName(refResources, ReferenceResource.LocalCompany);
                        employment.LocalJobTitles = GetRefResourceByName(refResources, ReferenceResource.LocalJobTitle);
                        employment.EmploymentTypes = GetRefResourceByName(refResources, ReferenceResource.EmploymentType);
                        employment.EmploymentStatuses = GetRefResourceByName(refResources, ReferenceResource.EmploymentStatus);
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