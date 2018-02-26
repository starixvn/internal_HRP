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
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class EmployeeRepository : IEmployeeRepository
    {
        public CreateEmployeeViewModel GetPersonalDetails(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var employee = multi.ReadFirstOrDefault<CreateEmployeeViewModel>();

                    if (employee != null && employee.EmployeeId > 0)
                    {
                        var emergencyContact = multi.ReadFirstOrDefault<EmergencyContact>();
                        var genderList = multi.Read<ReferenceResourceModel>();
                        var nationalityList = multi.Read<ReferenceResourceModel>();
                        var maritalStatusList = multi.Read<ReferenceResourceModel>();
                        var noOfChildrenList = multi.Read<ReferenceResourceModel>();

                        employee.EmergencyContact = emergencyContact;
                        employee.GenderList = GetSelectList(genderList);
                        employee.NationalityList = GetSelectList(nationalityList);
                        employee.MaritalStatusList = GetSelectList(maritalStatusList);
                        employee.NoOfChildrenList = GetSelectList(noOfChildrenList);

                        return employee;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

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

                    if (employment != null && employment.EmployeeId > 0)
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
                        employment.CompanyChangeReasons = GetRefResourceByName(refResources, ReferenceResource.CompanyBVIChangeReason);
                        employment.LocalCompanyChangeReasons = GetRefResourceByName(refResources, ReferenceResource.CompanyLocalChangeReason);
                        employment.JobTitleChangeReasons = GetRefResourceByName(refResources, ReferenceResource.JobTitleChangeReason);
                        employment.DateCommencementChangeReasons = GetRefResourceByName(refResources, ReferenceResource.DateCommencementChangeReason);
                        employment.IDNoChangeReasons = GetRefResourceByName(refResources, ReferenceResource.EmployeeIDChangeReason);
                        employment.JobCategoryChangeReasons = GetRefResourceByName(refResources, ReferenceResource.JobCategoryChangeReason);
                        employment.JobGradeChangeReasons = GetRefResourceByName(refResources, ReferenceResource.JobGradeChangeReason);
                        employment.DivisionChangeReasons = GetRefResourceByName(refResources, ReferenceResource.DivisionChangeReason);
                        employment.DepartmentChangeReasons = GetRefResourceByName(refResources, ReferenceResource.DepartmentChangeReason);
                        employment.CostCenterChangeReasons = GetRefResourceByName(refResources, ReferenceResource.CostCenterChangeReason);
                        employment.SectionChangeReasons = GetRefResourceByName(refResources, ReferenceResource.SectionChangeReason);

                        return employment;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public EmployeeManagementViewModel GetDataFilterForIndexPage(ICriteria criteria)
        {
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                using (IMultiResultReader multi = db.QueryMultiple(criteria))
                {
                    var viewModel = multi.ReadFirst<EmployeeManagementViewModel>();
                    var companies = multi.Read<MultiSelectViewModel>();
                    var divisions = multi.Read<MultiSelectViewModel>();
                    var departments = multi.Read<MultiSelectViewModel>();
                    var employmentStatuses = multi.Read<MultiSelectViewModel>();

                    viewModel.HierarchyCompanies = companies.Select(company => new MultiSelectViewModel
                    {
                        Value = company.Value,
                        Text = company.Text,
                        Checked = company.Checked,
                        Items = divisions.Where(division => division.ParentId == company.Value).Select(division => new MultiSelectViewModel
                        {
                            Value = division.Value,
                            Text = division.Text,
                            Checked = division.Checked,
                            Items = departments.Where(dept => dept.ParentId == division.Value).Select(dept => new MultiSelectViewModel
                            {
                                Value = dept.Value,
                                Text = dept.Text,
                                Checked = dept.Checked
                            })
                        })
                    });

                    viewModel.EmploymentStatuses = employmentStatuses;

                    return viewModel;
                }
            }
        }

        private static IEnumerable<ReferenceResourceModel> GetRefResourceByName(IEnumerable<ReferenceResourceModel> refResources, string refName)
        {
            return refResources.Where(x => x.RefName == refName);
        }

        private static SelectList GetSelectList(IEnumerable<ReferenceResourceModel> refResource)
        {
            return new SelectList(refResource, "ResourceId", "Value");
        }
    }
}