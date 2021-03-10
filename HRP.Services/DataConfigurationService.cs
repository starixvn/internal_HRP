namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Criteria;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.CostCenter;
    using HRP.Criteria.Currency;
    using HRP.Criteria.Department;
    using HRP.Criteria.GradeLevel;
    using HRP.Criteria.JobCategory;
    using HRP.Criteria.JobGrade;
    using HRP.Criteria.RefResource;
    using HRP.Criteria.Section;
    using HRP.Models;
    using HRP.Models.Currency;
    using HRP.Models.DataConfiguration;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class DataConfigurationService : IDataConfigurationService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public DataConfigurationService(
            IBaseService baseService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        public IEnumerable<Field> GetDataForIndexPage()
        {
            return FieldsDictionary.Fields.Values.OrderBy(x => x.FieldName);
        }

        #region CompanyBVI

        public async Task<IEnumerable<SimpleItem>> GetAllCompanyBVIAsync()
        {
            var companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria());

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

        public async Task<CommandResult> CreateCompanyBVIAsync(SimpleItem item)
        {
            var criteria = new CreateCompanyCriteria
            {
                Code = item.ItemName,
                Name = item.Description,
                IsActive = item.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<SimpleItem> GetCompanyBVIByIdAsync(int id)
        {
            var criteria = new GetCompanyByIdCriteria
            {
                CompanyId = id
            };

            var company = await baseService.GetDataEntityAsync<Company>(criteria);

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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        #endregion CompanyBVI

        #region Division

        public async Task<IEnumerable<Division>> GetAllDivisionAsync()
        {
            var divisions = await baseService.GetDataAsync<Division>(new GetAllDivisionCriteria());

            return divisions;
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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<Division> GetDivisionByIdAsync(int id)
        {
            var division = await baseService.GetDataEntityAsync<Division>(new GetDivisionByIdCriteria { DivisionId = id });

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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        #endregion Division

        #region Department

        public async Task<IEnumerable<Department>> GetAllDepartmentAsync()
        {
            var departments = await baseService.GetDataAsync<Department>(new GetAllDepartmentCriteria());

            return departments;
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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            var department = await baseService.GetDataEntityAsync<Department>(new GetDepartmentByIdCriteria { DepartmentId = id });

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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        #endregion Department

        #region Cost Center

        public async Task<IEnumerable<CostCenter>> GetAllCostCenterAsync()
        {
            var costCenters = await baseService.GetDataAsync<CostCenter>(new GetAllCostCenterCriteria());

            return costCenters;
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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<CostCenter> GetCostCenterByIdAsync(int id)
        {
            var costCenter = await baseService.GetDataEntityAsync<CostCenter>(new GetCostCenterByIdCriteria { CostCenterId = id });

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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        #endregion Cost Center

        #region Section

        public async Task<IEnumerable<Section>> GetAllSectionAsync()
        {
            var sections = await baseService.GetDataAsync<Section>(new GetAllSectionCriteria());

            return sections;
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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<Section> GetSectionByIdAsync(int id)
        {
            var section = await baseService.GetDataEntityAsync<Section>(new GetSectionByIdCriteria { SectionId = id });

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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        #endregion Section

        #region Job Category

        public async Task<IEnumerable<SimpleItem>> GetAllJobCategoryAsync()
        {
            var jobCategories = await baseService.GetDataAsync<JobCategory>(new GetAllJobCategoryCriteria());

            if (jobCategories.Any())
            {
                var result = jobCategories.Select(c => new SimpleItem
                {
                    ItemId = c.JobCategoryId,
                    ItemName = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                }).ToList();

                return result;
            }

            return new List<SimpleItem>();
        }

        public async Task<CommandResult> CreateJobCategoryAsync(SimpleItem item)
        {
            var criteria = new CreateJobCategoryCriteria
            {
                Name = item.ItemName,
                Description = item.Description,
                IsActive = item.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<SimpleItem> GetJobCategoryByIdAsync(int id)
        {
            var criteria = new GetJobCategoryByIdCriteria
            {
                JobCategoryId = id
            };

            var jobCategory = await baseService.GetDataEntityAsync<JobCategory>(criteria);

            if (jobCategory != null)
            {
                return new SimpleItem
                {
                    ItemId = jobCategory.JobCategoryId,
                    ItemName = jobCategory.Name,
                    Description = jobCategory.Description,
                    IsActive = jobCategory.IsActive
                };
            }

            return null;
        }

        public async Task<CommandResult> UpdateJobCategoryAsync(SimpleItem item)
        {
            var criteria = new UpdateJobCategoryCriteria
            {
                JobCategoryId = item.ItemId,
                Name = item.ItemName,
                Description = item.Description,
                IsActive = item.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        #endregion Job Category

        #region Grade Level

        public async Task<IEnumerable<GradeLevel>> GetAllGradeLevelAsync()
        {
            var gradeLevelList = await baseService.GetDataAsync<GradeLevel>(new GetAllGradeLevelCriteria());

            return gradeLevelList;
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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<GradeLevel> GetGradeLevelByIdAsync(int id)
        {
            var gradeLevel = await baseService.GetDataEntityAsync<GradeLevel>(new GetGradeLevelByIdCriteria { GradeLevelId = id });

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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        #endregion Grade Level

        #region Job Grade

        public async Task<IEnumerable<JobGrade>> GetAllJobGradeAsync()
        {
            var jobGradeList = await baseService.GetDataAsync<JobGrade>(new GetAllJobGradeCriteria());

            return jobGradeList;
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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<JobGrade> GetJobGradeByIdAsync(int id)
        {
            var jobGrade = await baseService.GetDataEntityAsync<JobGrade>(new GetJobGradeByIdCriteria { JobGradeId = id });

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

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        #endregion Job Grade

        #region RefResource Item

        public async Task<IEnumerable<SimpleItem>> GetAllRefResourceItemAsync(string refName)
        {
            var items = await baseService.GetDataAsync<ReferenceResourceModel>(new GetRefResourceByNamesCriteria { RefNames = refName });

            if (items.Any())
            {
                var result = items.Select(c => new SimpleItem
                {
                    ItemId = c.ResourceId.Value,
                    ItemName = c.Value,
                    Description = c.Description,
                    IsActive = c.IsActive
                }).ToList();

                return result;
            }

            return new List<SimpleItem>();
        }

        public async Task<CommandResult> CreateRefResourceItemAsync(string refName, SimpleItem item)
        {
            var criteria = new CreateRefResourceCriteria
            {
                RefName = refName,
                Value = item.ItemName,
                Description = item.Description,
                IsActive = item.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<SimpleItem> GetRefResourceByNameAndIdAsync(string refName, int resourceId)
        {
            var criteria = new GetRefResourceByNameAndIdCriteria
            {
                ResourceId = resourceId,
                RefName = refName
            };

            var item = await baseService.GetDataEntityAsync<ReferenceResourceModel>(criteria);

            if (item != null)
            {
                return new SimpleItem
                {
                    ItemId = item.ResourceId.Value,
                    ItemName = item.Value,
                    Description = item.Description,
                    IsActive = item.IsActive
                };
            }

            return null;
        }

        public async Task<CommandResult> UpdateRefResourceItemAsync(string refName, SimpleItem item)
        {
            var criteria = new UpdateRefResourceItemCriteria
            {
                RefName = refName,
                ResourceId = item.ItemId,
                Value = item.ItemName,
                Description = item.Description,
                IsActive = item.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        #endregion RefResource Item

        #region Currency

        public async Task<IEnumerable<SimpleItem>> GetAllCurrencyAsync()
        {
            var currencies = await baseService.GetDataAsync<Currency>(new GetAllCurrencyCriteria());

            if (currencies.Any())
            {
                var result = currencies.Select(c => new SimpleItem
                {
                    ItemId = c.CurrencyId,
                    ItemName = c.CurrencyCode,
                    Description = c.CurrencyName,
                    IsActive = c.IsActive
                }).ToList();

                return result;
            }

            return new List<SimpleItem>();
        }

        public async Task<CommandResult> CreateCurrencyAsync(SimpleItem item)
        {
            var criteria = new CreateCurrencyCriteria
            {
                CurrencyCode = item.ItemName,
                CurrencyName = item.Description,
                IsActive = item.IsActive,
                CreatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        public async Task<SimpleItem> GetCurrencyByIdAsync(int id)
        {
            var criteria = new GetCurrencyByIdCriteria
            {
                CurrencyId = id
            };

            var currency = await baseService.GetDataEntityAsync<Currency>(criteria);

            if (currency != null)
            {
                return new SimpleItem
                {
                    ItemId = currency.CurrencyId,
                    ItemName = currency.CurrencyCode,
                    Description = currency.CurrencyName,
                    IsActive = currency.IsActive
                };
            }

            return null;
        }

        public async Task<CommandResult> UpdateCurrencyAsync(SimpleItem item)
        {
            var criteria = new UpdateCurrencyCriteria
            {
                CurrencyId = item.ItemId,
                CurrencyCode = item.ItemName,
                CurrencyName = item.Description,
                IsActive = item.IsActive,
                UpdatedBy = sessionService.UserId
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria);

            return commandResult;
        }

        #endregion Currency

        #region Common

        public async Task<SelectList> GetCompanyBVISelectListAsync()
        {
            var companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria());

            return new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
        }

        #endregion Common
    }
}