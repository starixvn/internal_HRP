namespace HRP.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
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

        Task<bool> IsExistedDivisionAsync(Division division);

        Task<CommandResult> CreateDivisionAsync(Division division);

        Task<Division> GetDivisionByIdAsync(int id);

        Task<CommandResult> UpdateDivisionAsync(Division division);

        Task<IEnumerable<Department>> GetAllDepartmentAsync();

        Task<bool> IsExistedDepartmentAsync(Department department);

        Task<CommandResult> CreateDepartmentAsync(Department department);

        Task<Department> GetDepartmentByIdAsync(int id);

        Task<CommandResult> UpdateDepartmentAsync(Department department);

        Task<IEnumerable<CostCenter>> GetAllCostCenterAsync();

        Task<bool> IsExistedCostCenterAsync(CostCenter costCenter);

        Task<CommandResult> CreateCostCenterAsync(CostCenter costCenter);

        Task<CostCenter> GetCostCenterByIdAsync(int id);

        Task<CommandResult> UpdateCostCenterAsync(CostCenter costCenter);

        Task<IEnumerable<Section>> GetAllSectionAsync();

        Task<bool> IsExistedSectionAsync(Section section);

        Task<CommandResult> CreateSectionAsync(Section section);

        Task<Section> GetSectionByIdAsync(int id);

        Task<CommandResult> UpdateSectionAsync(Section section);

        Task<IEnumerable<GradeLevel>> GetAllGradeLevelAsync();

        Task<bool> IsExistedGradeLevelAsync(GradeLevel gradeLevel);

        Task<CommandResult> CreateGradeLevelAsync(GradeLevel gradeLevel);

        Task<GradeLevel> GetGradeLevelByIdAsync(int id);

        Task<CommandResult> UpdateGradeLevelAsync(GradeLevel gradeLevel);

        //// Common funcs
        Task<SelectList> GetDivisionSelectListByCompanyIdAsync(int companyId);

        Task<SelectList> GetDepartmentSelectListByDivisionIdAsync(int divisionId);

        Task<SelectList> GetCostCenterSelectListByDepartmentIdAsync(int departmentId);

        Task<SelectList> GetSectionSelectListByCostCenterIdAsync(int costCenterId);

        Task<SelectList> GetCompanyBVISelectListAsync();

        Task<SelectList> GetJobCategorySelectListAsync();
    }
}