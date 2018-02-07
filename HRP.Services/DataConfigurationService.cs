namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Constants;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.City;
    using HRP.Criteria.CostCenter;
    using HRP.Criteria.Department;
    using HRP.Criteria.GradeLevel;
    using HRP.Criteria.JobGrade;
    using HRP.Criteria.Office;
    using HRP.Criteria.Section;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Resources;
    using HRP.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class DataConfigurationService : IDataConfigurationService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public DataConfigurationService(IBaseService baseService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        public IEnumerable<Field> GetDataForIndexPage()
        {
            var fieldList = new List<Field>
            {
                new Field
                {
                    FieldId = (int)FieldType.CompanyBVI,
                    FieldName = ViewLang.CompanyBVI,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllCompanyBVI
                },

                new Field
                {
                    FieldId = (int)FieldType.CompanyLocal,
                    FieldName = ViewLang.CompanyLocal,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllCompanyLocal
                },

                new Field
                {
                    FieldId = (int)FieldType.Division,
                    FieldName = ViewLang.Division,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllDivision
                },

                new Field
                {
                    FieldId = (int)FieldType.Department,
                    FieldName = ViewLang.Department,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllDepartment
                },

                new Field
                {
                    FieldId = (int)FieldType.CostCenter,
                    FieldName = ViewLang.CostCenter,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllCostCenter
                },

                new Field
                {
                    FieldId = (int)FieldType.GradeLevel,
                    FieldName = ViewLang.GradeLevel,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllGradeLevel
                },

                new Field
                {
                    FieldId = (int)FieldType.JobGrade,
                    FieldName = ViewLang.JobGrade,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllJobGrade
                },

                new Field
                {
                    FieldId = (int)FieldType.City,
                    FieldName = ViewLang.City,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllCity
                },

                new Field
                {
                    FieldId = (int)FieldType.Office,
                    FieldName = ViewLang.Office,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllOffice
                }
            };

            return fieldList.OrderBy(x => x.FieldName);
        }

        #region CompanyBVI

        public async Task<IEnumerable<SimpleItem>> GetAllCompanyBVIAsync()
        {
            var companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria()).ConfigureAwait(false);

            if (companies.Any())
            {
                var result = companies.Select(c => new SimpleItem
                {
                    ItemId = c.CompanyId,
                    ItemName = c.Code,
                    Description = c.Name,
                    IsActive = c.IsActive
                }).ToList();

                return result;
            }

            return new List<SimpleItem>();
        }

        public async Task<bool> IsExistedCompanyBVIAsync(SimpleItem item)
        {
            var isExisted = await baseService.GetDataEntityAsync<bool>(new CheckCompanyExistsCriteria { CompanyId = item.ItemId, Code = item.ItemName }).ConfigureAwait(false);

            return isExisted;
        }

        public async Task<CommandResult> CreateCompanyBVIAsync(SimpleItem item)
        {
            var criteria = new CreateCompanyCriteria
            {
                Code = item.ItemName,
                Name = item.Description,
                IsActive = item.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = string.Format(Message.ItemCreatedSuccessful, criteria.Code);
            }

            return commandResult;
        }

        public async Task<SimpleItem> GetCompanyBVIByIdAsync(int id)
        {
            var criteria = new GetCompanyByIdCriteria
            {
                CompanyId = id
            };

            var company = await baseService.GetDataEntityAsync<Company>(criteria).ConfigureAwait(false);

            if (company != null)
            {
                return new SimpleItem
                {
                    ItemId = company.CompanyId,
                    ItemName = company.Code,
                    Description = company.Name,
                    IsActive = company.IsActive
                };
            }

            return null;
        }

        public async Task<CommandResult> UpdateCompanyBVIAsync(SimpleItem item)
        {
            var criteria = new UpdateCompanyCriteria
            {
                CompanyId = item.ItemId,
                Code = item.ItemName,
                Name = item.Description,
                IsActive = item.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = Message.ItemUpdatedSuccessful;
            }

            return commandResult;
        }

        #endregion CompanyBVI

        #region Division

        public async Task<IEnumerable<Division>> GetAllDivisionAsync()
        {
            var divisions = await baseService.GetDataAsync<Division>(new GetAllDivisionCriteria()).ConfigureAwait(false);

            return divisions;
        }

        public async Task<bool> IsExistedDivisionAsync(Division division)
        {
            var criteria = new CheckDivisionExistsCriteria
            {
                DivisionId = division.DivisionId,
                Code = division.Code,
                CompanyId = division.CompanyId
            };
            var isExisted = await baseService.GetDataEntityAsync<bool>(criteria).ConfigureAwait(false);

            return isExisted;
        }

        public async Task<CommandResult> CreateDivisionAsync(Division division)
        {
            var criteria = new CreateDivisionCriteria
            {
                Code = division.Code,
                Name = division.Name,
                CompanyId = division.CompanyId,
                IsActive = division.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = string.Format(Message.ItemCreatedSuccessful, criteria.Code);
            }

            return commandResult;
        }

        public async Task<Division> GetDivisionByIdAsync(int id)
        {
            var division = await baseService.GetDataEntityAsync<Division>(new GetDivisionByIdCriteria { DivisionId = id }).ConfigureAwait(false);

            return division;
        }

        public async Task<CommandResult> UpdateDivisionAsync(Division division)
        {
            var criteria = new UpdateDivisionCriteria
            {
                DivisionId = division.DivisionId,
                Code = division.Code,
                Name = division.Name,
                IsActive = division.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = Message.ItemUpdatedSuccessful;
            }

            return commandResult;
        }

        #endregion Division

        #region Department

        public async Task<IEnumerable<Department>> GetAllDepartmentAsync()
        {
            var departments = await baseService.GetDataAsync<Department>(new GetAllDepartmentCriteria()).ConfigureAwait(false);

            return departments;
        }

        public async Task<bool> IsExistedDepartmentAsync(Department department)
        {
            var criteria = new CheckDepartmentExistsCriteria
            {
                DepartmentId = department.DepartmentId,
                Code = department.Code,
                DivisionId = department.DivisionId
            };

            var isExisted = await baseService.GetDataEntityAsync<bool>(criteria).ConfigureAwait(false);

            return isExisted;
        }

        public async Task<CommandResult> CreateDepartmentAsync(Department department)
        {
            var criteria = new CreateDepartmentCriteria
            {
                Code = department.Code,
                Name = department.Name,
                DivisionId = department.DivisionId,
                IsActive = department.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = string.Format(Message.ItemCreatedSuccessful, criteria.Code);
            }

            return commandResult;
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            var department = await baseService.GetDataEntityAsync<Department>(new GetDepartmentByIdCriteria { DepartmentId = id }).ConfigureAwait(false);

            return department;
        }

        public async Task<CommandResult> UpdateDepartmentAsync(Department department)
        {
            var criteria = new UpdateDepartmentCriteria
            {
                DepartmentId = department.DepartmentId,
                Code = department.Code,
                Name = department.Name,
                IsActive = department.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = Message.ItemUpdatedSuccessful;
            }

            return commandResult;
        }

        #endregion Department

        #region Cost Center

        public async Task<IEnumerable<CostCenter>> GetAllCostCenterAsync()
        {
            var costCenters = await baseService.GetDataAsync<CostCenter>(new GetAllCostCenterCriteria()).ConfigureAwait(false);

            return costCenters;
        }

        public async Task<bool> IsExistedCostCenterAsync(CostCenter costCenter)
        {
            var criteria = new CheckCostCenterExistsCriteria
            {
                CostCenterId = costCenter.CostCenterId,
                Code = costCenter.Code,
                DepartmentId = costCenter.DepartmentId
            };

            var isExisted = await baseService.GetDataEntityAsync<bool>(criteria).ConfigureAwait(false);

            return isExisted;
        }

        public async Task<CommandResult> CreateCostCenterAsync(CostCenter costCenter)
        {
            var criteria = new CreateCostCenterCriteria
            {
                Code = costCenter.Code,
                Name = costCenter.Name,
                DepartmentId = costCenter.DepartmentId,
                IsActive = costCenter.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = string.Format(Message.ItemCreatedSuccessful, criteria.Code);
            }

            return commandResult;
        }

        public async Task<CostCenter> GetCostCenterByIdAsync(int id)
        {
            var costCenter = await baseService.GetDataEntityAsync<CostCenter>(new GetCostCenterByIdCriteria { CostCenterId = id }).ConfigureAwait(false);

            return costCenter;
        }

        public async Task<CommandResult> UpdateCostCenterAsync(CostCenter costCenter)
        {
            var criteria = new UpdateCostCenterCriteria
            {
                CostCenterId = costCenter.CostCenterId,
                Code = costCenter.Code,
                Name = costCenter.Name,
                IsActive = costCenter.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = Message.ItemUpdatedSuccessful;
            }

            return commandResult;
        }

        #endregion Cost Center

        #region Section

        public async Task<IEnumerable<Section>> GetAllSectionAsync()
        {
            var sections = await baseService.GetDataAsync<Section>(new GetAllSectionCriteria()).ConfigureAwait(false);

            return sections;
        }

        public async Task<bool> IsExistedSectionAsync(Section section)
        {
            var criteria = new CheckSectionExistsCriteria
            {
                SectionId = section.SectionId,
                Code = section.Code,
                CostCenterId = section.CostCenterId
            };

            var isExisted = await baseService.GetDataEntityAsync<bool>(criteria).ConfigureAwait(false);

            return isExisted;
        }

        public async Task<CommandResult> CreateSectionAsync(Section section)
        {
            var criteria = new CreateSectionCriteria
            {
                Code = section.Code,
                Name = section.Name,
                CostCenterId = section.CostCenterId,
                IsActive = section.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = string.Format(Message.ItemCreatedSuccessful, criteria.Code);
            }

            return commandResult;
        }

        public async Task<Section> GetSectionByIdAsync(int id)
        {
            var section = await baseService.GetDataEntityAsync<Section>(new GetSectionByIdCriteria { SectionId = id }).ConfigureAwait(false);

            return section;
        }

        public async Task<CommandResult> UpdateSectionAsync(Section section)
        {
            var criteria = new UpdateSectionCriteria
            {
                SectionId = section.SectionId,
                Code = section.Code,
                Name = section.Name,
                IsActive = section.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = Message.ItemUpdatedSuccessful;
            }

            return commandResult;
        }

        #endregion Section

        #region Grade Level

        public async Task<IEnumerable<GradeLevel>> GetAllGradeLevelAsync()
        {
            var gradeLevelList = await baseService.GetDataAsync<GradeLevel>(new GetAllGradeLevelCriteria()).ConfigureAwait(false);

            return gradeLevelList;
        }

        public async Task<bool> IsExistedGradeLevelAsync(GradeLevel gradeLevel)
        {
            var criteria = new CheckGradeLevelExistsCriteria
            {
                GradeLevelId = gradeLevel.GradeLevelId,
                Name = gradeLevel.Name,
                JobCategoryId = gradeLevel.JobCategoryId
            };
            var isExisted = await baseService.GetDataEntityAsync<bool>(criteria).ConfigureAwait(false);

            return isExisted;
        }

        public async Task<CommandResult> CreateGradeLevelAsync(GradeLevel gradeLevel)
        {
            var criteria = new CreateGradeLevelCriteria
            {
                Name = gradeLevel.Name,
                Description = gradeLevel.Description,
                JobCategoryId = gradeLevel.JobCategoryId,
                IsActive = gradeLevel.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = string.Format(Message.ItemCreatedSuccessful, criteria.Name);
            }

            return commandResult;
        }

        public async Task<GradeLevel> GetGradeLevelByIdAsync(int id)
        {
            var gradeLevel = await baseService.GetDataEntityAsync<GradeLevel>(new GetGradeLevelByIdCriteria { GradeLevelId = id }).ConfigureAwait(false);

            return gradeLevel;
        }

        public async Task<CommandResult> UpdateGradeLevelAsync(GradeLevel gradeLevel)
        {
            var criteria = new UpdateGradeLevelCriteria
            {
                GradeLevelId = gradeLevel.GradeLevelId,
                Name = gradeLevel.Name,
                Description = gradeLevel.Description,
                IsActive = gradeLevel.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = Message.ItemUpdatedSuccessful;
            }

            return commandResult;
        }

        #endregion Grade Level

        #region Job Grade

        public async Task<IEnumerable<JobGrade>> GetAllJobGradelAsync()
        {
            var jobGradeList = await baseService.GetDataAsync<JobGrade>(new GetAllJobGradeCriteria()).ConfigureAwait(false);

            return jobGradeList;
        }

        public async Task<bool> IsExistedJobGradeAsync(JobGrade jobGrade)
        {
            var criteria = new CheckJobGradeExistsCriteria
            {
                JobGradeId = jobGrade.JobGradeId,
                Name = jobGrade.Name,
                GradeLevelId = jobGrade.GradeLevelId
            };
            var isExisted = await baseService.GetDataEntityAsync<bool>(criteria).ConfigureAwait(false);

            return isExisted;
        }

        public async Task<CommandResult> CreateJobGradeAsync(JobGrade jobGrade)
        {
            var criteria = new CreateJobGradeCriteria
            {
                Name = jobGrade.Name,
                Description = jobGrade.Description,
                GradeLevelId = jobGrade.GradeLevelId,
                IsActive = jobGrade.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = string.Format(Message.ItemCreatedSuccessful, criteria.Name);
            }

            return commandResult;
        }

        public async Task<JobGrade> GetJobGradeByIdAsync(int id)
        {
            var jobGrade = await baseService.GetDataEntityAsync<JobGrade>(new GetJobGradeByIdCriteria { JobGradeId = id }).ConfigureAwait(false);

            return jobGrade;
        }

        public async Task<CommandResult> UpdateJobGradeAsync(JobGrade jobGrade)
        {
            var criteria = new UpdateJobGradeCriteria
            {
                JobGradeId = jobGrade.JobGradeId,
                Name = jobGrade.Name,
                Description = jobGrade.Description,
                IsActive = jobGrade.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = Message.ItemUpdatedSuccessful;
            }

            return commandResult;
        }

        #endregion Job Grade

        #region City

        public async Task<IEnumerable<City>> GetAllCityAsync()
        {
            var cities = await baseService.GetDataAsync<City>(new GetAllCityCriteria()).ConfigureAwait(false);

            return cities;
        }

        public async Task<bool> IsExistedCityAsync(City city)
        {
            var criteria = new CheckCityExistsCriteria
            {
                CityId = city.CityId,
                Code = city.Code,
                CountryId = city.CountryId
            };
            var isExisted = await baseService.GetDataEntityAsync<bool>(criteria).ConfigureAwait(false);

            return isExisted;
        }

        public async Task<CommandResult> CreateCityAsync(City city)
        {
            var criteria = new CreateCityCriteria
            {
                Code = city.Code,
                Name = city.Name,
                CountryId = city.CountryId,
                IsActive = city.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = string.Format(Message.ItemCreatedSuccessful, criteria.Code);
            }

            return commandResult;
        }

        public async Task<City> GetCityByIdAsync(int id)
        {
            var city = await baseService.GetDataEntityAsync<City>(new GetCityByIdCriteria { CityId = id }).ConfigureAwait(false);

            return city;
        }

        public async Task<CommandResult> UpdateCityAsync(City city)
        {
            var criteria = new UpdateCityCriteria
            {
                CityId = city.CityId,
                Code = city.Code,
                Name = city.Name,
                IsActive = city.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = Message.ItemUpdatedSuccessful;
            }

            return commandResult;
        }

        #endregion City

        #region Office

        public async Task<IEnumerable<Office>> GetAllOfficeAsync()
        {
            var offices = await baseService.GetDataAsync<Office>(new GetAllOfficeCriteria()).ConfigureAwait(false);

            return offices;
        }

        public async Task<bool> IsExistedOfficeAsync(Office office)
        {
            var criteria = new CheckOfficeExistsCriteria
            {
                OfficeId = office.OfficeId,
                Code = office.Code,
                CityId = office.CityId
            };
            var isExisted = await baseService.GetDataEntityAsync<bool>(criteria).ConfigureAwait(false);

            return isExisted;
        }

        public async Task<CommandResult> CreateOfficeAsync(Office office)
        {
            var criteria = new CreateOfficeCriteria
            {
                Code = office.Code,
                Name = office.Name,
                CityId = office.CityId,
                IsActive = office.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = string.Format(Message.ItemCreatedSuccessful, criteria.Code);
            }

            return commandResult;
        }

        public async Task<Office> GetOfficeByIdAsync(int id)
        {
            var office = await baseService.GetDataEntityAsync<Office>(new GetOfficeByIdCriteria { OfficeId = id }).ConfigureAwait(false);

            return office;
        }

        public async Task<CommandResult> UpdateOfficeAsync(Office office)
        {
            var criteria = new UpdateOfficeCriteria
            {
                OfficeId = office.OfficeId,
                Code = office.Code,
                Name = office.Name,
                IsActive = office.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            if (commandResult.IsSuccessful)
            {
                commandResult.Message = Message.ItemUpdatedSuccessful;
            }

            return commandResult;
        }

        #endregion Office

        #region Common

        public async Task<SelectList> GetDivisionSelectListByCompanyIdAsync(int companyId)
        {
            var criteria = new GetDivisionByCompanyIdCriteria { CompanyId = companyId };
            var divisions = await baseService.GetDataAsync<Division>(criteria).ConfigureAwait(false);

            return new SelectList(divisions, nameof(Division.DivisionId), nameof(Division.Code));
        }

        public async Task<SelectList> GetDepartmentSelectListByDivisionIdAsync(int divisionId)
        {
            var criteria = new GetDepartmentByDivisionIdCriteria { DivisionId = divisionId };
            var departments = await baseService.GetDataAsync<Department>(criteria).ConfigureAwait(false);

            return new SelectList(departments, nameof(Department.DepartmentId), nameof(Department.Code));
        }

        public async Task<SelectList> GetCostCenterSelectListByDepartmentIdAsync(int departmentId)
        {
            var criteria = new GetCostCenterDepartmentIdCriteria { DepartmentId = departmentId };
            var costCenters = await baseService.GetDataAsync<CostCenter>(criteria).ConfigureAwait(false);

            return new SelectList(costCenters, nameof(CostCenter.CostCenterId), nameof(CostCenter.Code));
        }

        public async Task<SelectList> GetSectionSelectListByCostCenterIdAsync(int costCenterId)
        {
            var criteria = new GetSectionByCostCenterIdCriteria { CostCenterId = costCenterId };
            var sections = await baseService.GetDataAsync<Section>(criteria).ConfigureAwait(false);

            return new SelectList(sections, nameof(Section.SectionId), nameof(Section.Code));
        }

        public async Task<SelectList> GetGradeLevelSelectListByJobCateogryIdAsync(int jobCategoryId)
        {
            var criteria = new GetGradeLevelByJobCategoryIdCriteria { JobCategoryId = jobCategoryId };
            var gradeLevelList = await baseService.GetDataAsync<GradeLevel>(criteria).ConfigureAwait(false);

            return new SelectList(gradeLevelList, nameof(GradeLevel.GradeLevelId), nameof(GradeLevel.Name));
        }

        public async Task<SelectList> GetCitySelectListByCountryIdAsync(int countryId)
        {
            var criteria = new GetCityByCountryIdCriteria { CountryId = countryId };
            var cityList = await baseService.GetDataAsync<City>(criteria).ConfigureAwait(false);

            return new SelectList(cityList, nameof(City.CityId), nameof(City.Code));
        }

        public async Task<SelectList> GetCompanyBVISelectListAsync()
        {
            var companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria()).ConfigureAwait(false);

            return new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
        }

        public async Task<SelectList> GetJobCategorySelectListAsync()
        {
            var jobCategoryList = await baseService.GetDataAsync<JobCategory>(new GetAllJobCategoryCriteria()).ConfigureAwait(false);

            return new SelectList(jobCategoryList, nameof(JobCategory.JobCategoryId), nameof(JobCategory.Name));
        }

        public async Task<SelectList> GetCountrySelectListAsync()
        {
            var countryList = await baseService.GetDataAsync<Country>(new GetAllCountryCriteria()).ConfigureAwait(false);

            return new SelectList(countryList, nameof(Country.CountryId), nameof(Country.Code));
        }

        #endregion Common
    }
}