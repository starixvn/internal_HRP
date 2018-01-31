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
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Models.DataConfiguration.Department;
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
            var fieldList = new List<Field>();
            fieldList.Add(new Field
            {
                FieldId = (int)FieldType.CompanyBVI,
                FieldName = ViewLang.CompanyBVI,
                LinkGetAllItem = DataConfigurationActionLink.GetAllCompanyBVI
            });

            fieldList.Add(new Field
            {
                FieldId = (int)FieldType.CompanyLocal,
                FieldName = ViewLang.CompanyLocal,
                LinkGetAllItem = DataConfigurationActionLink.GetAllCompanyLocal
            });

            fieldList.Add(new Field
            {
                FieldId = (int)FieldType.Division,
                FieldName = ViewLang.Division,
                LinkGetAllItem = DataConfigurationActionLink.GetAllDivision
            });

            fieldList.Add(new Field
            {
                FieldId = (int)FieldType.Department,
                FieldName = ViewLang.Department,
                LinkGetAllItem = DataConfigurationActionLink.GetAllDepartment
            });

            fieldList.Add(new Field
            {
                FieldId = (int)FieldType.CostCenter,
                FieldName = ViewLang.CostCenter,
                LinkGetAllItem = DataConfigurationActionLink.GetAllCostCenter
            });

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

        #region Common

        public async Task<SelectList> GetDivisionSelectListByCompanyIdAsync(int companyId)
        {
            var criteria = new GetDivisionByCompanyIdCriteria { CompanyId = companyId };
            var divisions = await baseService.GetDataAsync<Division>(criteria).ConfigureAwait(false);

            return new SelectList(divisions, nameof(Division.DivisionId), nameof(Division.Code));
        }

        private async Task<SelectList> GetCompanyBVISelectListAsync()
        {
            var companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria()).ConfigureAwait(false);

            return new SelectList(companies, nameof(Company.CompanyId), nameof(Company.Code));
        }

        #endregion Common
    }
}