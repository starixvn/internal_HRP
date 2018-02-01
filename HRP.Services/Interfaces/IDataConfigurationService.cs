namespace HRP.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Models.DataConfiguration.CostCenter;
    using HRP.Models.DataConfiguration.Department;
    using HRP.Models.DataConfiguration.Division;
    using HRP.Models.DataConfiguration.GradeLevel;
    using HRP.Models.DataConfiguration.Section;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface IDataConfigurationService
    {
        IEnumerable<Field> GetDataForIndexPage();

        Task<IEnumerable<SimpleItem>> GetAllCompanyBVIAsync();

        Task<bool> IsExistedCompanyBVIAsync(SimpleItem item);

        Task<SimpleItem> GetCompanyBVIByIdAsync(int id);

        Task<CommandResult> CreateCompanyBVIAsync(SimpleItem item);

        Task<CommandResult> UpdateCompanyBVIAsync(SimpleItem item);

        Task<IEnumerable<Division>> GetAllDivisionAsync();

        Task<CreateDivisionViewModel> GetDataForCreateDivisionAsync();

        Task<bool> IsExistedDivisionAsync(Division division);

        Task<CommandResult> CreateDivisionAsync(Division division);

        Task<Division> GetDivisionByIdAsync(int id);

        Task<CommandResult> UpdateDivisionAsync(Division division);

        Task<IEnumerable<Department>> GetAllDepartmentAsync();

        Task<CreateDepartmentViewModel> GetDataForCreateDepartmentAsync();

        Task<bool> IsExistedDepartmentAsync(Department department);

        Task<CommandResult> CreateDepartmentAsync(Department department);

        Task<Department> GetDepartmentByIdAsync(int id);

        Task<CommandResult> UpdateDepartmentAsync(Department department);

        Task<IEnumerable<CostCenter>> GetAllCostCenterAsync();

        Task<CreateCostCenterViewModel> GetDataForCreateCostCenterAsync();

        Task<bool> IsExistedCostCenterAsync(CostCenter costCenter);

        Task<CommandResult> CreateCostCenterAsync(CostCenter costCenter);

        Task<CostCenter> GetCostCenterByIdAsync(int id);

        Task<CommandResult> UpdateCostCenterAsync(CostCenter costCenter);

        Task<IEnumerable<Section>> GetAllSectionAsync();

        Task<CreateSectionViewModel> GetDataForCreateSectionAsync();

        Task<bool> IsExistedSectionAsync(Section section);

        Task<CommandResult> CreateSectionAsync(Section section);

        Task<Section> GetSectionByIdAsync(int id);

        Task<CommandResult> UpdateSectionAsync(Section section);

        Task<IEnumerable<GradeLevel>> GetAllGradeLevelAsync();

        Task<CreateGradeLevelViewModel> GetDataForCreateGradeLevelAsync();

        Task<bool> IsExistedGradeLevelAsync(GradeLevel gradeLevel);

        Task<CommandResult> CreateGradeLevelAsync(GradeLevel gradeLevel);

        Task<GradeLevel> GetGradeLevelByIdAsync(int id);

        Task<CommandResult> UpdateGradeLevelAsync(GradeLevel gradeLevel);

        //// Common funcs
        Task<SelectList> GetDivisionSelectListByCompanyIdAsync(int companyId);

        Task<SelectList> GetDepartmentSelectListByDivisionIdAsync(int divisionId);

        Task<SelectList> GetCostCenterSelectListByDepartmentIdAsync(int departmentId);

        Task<SelectList> GetSectionSelectListByCostCenterIdAsync(int costCenterId);
    }
}