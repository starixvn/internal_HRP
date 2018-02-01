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
    using HRP.Criteria.CostCenter;
    using HRP.Criteria.Department;
    using HRP.Criteria.Section;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Models.DataConfiguration.CostCenter;
    using HRP.Models.DataConfiguration.Department;
    using HRP.Models.DataConfiguration.Section;
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
                new Field                {
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
                    FieldId = (int)FieldType.Section,
                    FieldName = ViewLang.Section,
                    LinkGetAllItem = DataConfigurationActionLink.GetAllSection
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
                });

                return result;
            }

            return new List<SimpleItem>();
        }

        public async Task<bool> IsExistedCompanyBVIAsync(SimpleItem item)
        {
            var isExisted = (await baseService.GetDataAsync<int>(new CheckCompanyExistsCriteria { CompanyId = item.ItemId, Code = item.ItemName }).ConfigureAwait(false)).Any();

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
                    ItemId = id,
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

        public async Task<CreateDivisionViewModel> GetDataForCreateDivisionAsync()
        {
            var viewModel = new CreateDivisionViewModel
            {
                CompanyList = await GetCompanyBVISelectListAsync().ConfigureAwait(false)
            };

            return viewModel;
        }

        public async Task<bool> IsExistedDivisionAsync(Division division)
        {
            var criteria = new CheckDivisionExistsCriteria
            {
                DivisionId = division.DivisionId,
                Code = division.Code,
                CompanyId = division.CompanyId
            };
            var isExisted = (await baseService.GetDataAsync<int>(criteria).ConfigureAwait(false)).Any();

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

        public async Task<CreateDepartmentViewModel> GetDataForCreateDepartmentAsync()
        {
            var viewModel = new CreateDepartmentViewModel
            {
                CompanyList = await GetCompanyBVISelectListAsync().ConfigureAwait(false)
            };

            return viewModel;
        }

        public async Task<bool> IsExistedDepartmentAsync(Department department)
        {
            var criteria = new CheckDepartmentExistsCriteria
            {
                DepartmentId = department.DepartmentId,
                Code = department.Code,
                DivisionId = department.DivisionId
            };

            var isExisted = (await baseService.GetDataAsync<int>(criteria).ConfigureAwait(false)).Any();

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

        public async Task<CreateCostCenterViewModel> GetDataForCreateCostCenterAsync()
        {
            var viewModel = new CreateCostCenterViewModel
            {
                CompanyList = await GetCompanyBVISelectListAsync().ConfigureAwait(false)
            };

            return viewModel;
        }

        public async Task<bool> IsExistedCostCenterAsync(CostCenter costCenter)
        {
            var criteria = new CheckCostCenterExistsCriteria
            {
                CostCenterId = costCenter.CostCenterId,
                Code = costCenter.Code,
                DepartmentId = costCenter.DepartmentId
            };

            var isExisted = (await baseService.GetDataAsync<int>(criteria).ConfigureAwait(false)).Any();

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

        public async Task<CreateSectionViewModel> GetDataForCreateSectionAsync()
        {
            var viewModel = new CreateSectionViewModel
            {
                CompanyList = await GetCompanyBVISelectListAsync().ConfigureAwait(false)
            };

            return viewModel;
        }

        public async Task<bool> IsExistedSectionAsync(Section section)
        {
            var criteria = new CheckSectionExistsCriteria
            {
                SectionId = section.SectionId,
                Code = section.Code,
                CostCenterId = section.CostCenterId
            };

            var isExisted = (await baseService.GetDataAsync<int>(criteria).ConfigureAwait(false)).Any();

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

        private async Task<SelectList> GetCompanyBVISelectListAsync()
        {
            var companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria()).ConfigureAwait(false);

            return new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
        }

        #endregion Common
    }
}