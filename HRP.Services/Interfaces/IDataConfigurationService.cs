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

        Task<SimpleItem> GetCompanyBVIByIdAsync(int id);

        Task<CommandResult> CreateCompanyBVIAsync(SimpleItem item);

        Task<CommandResult> UpdateCompanyBVIAsync(SimpleItem item);

        Task<IEnumerable<Division>> GetAllDivisionAsync();

        Task<CommandResult> CreateDivisionAsync(Division division);

        Task<Division> GetDivisionByIdAsync(int id);

        Task<CommandResult> UpdateDivisionAsync(Division division);

        Task<IEnumerable<Department>> GetAllDepartmentAsync();

        Task<CommandResult> CreateDepartmentAsync(Department department);

        Task<Department> GetDepartmentByIdAsync(int id);

        Task<CommandResult> UpdateDepartmentAsync(Department department);

        Task<IEnumerable<CostCenter>> GetAllCostCenterAsync();

        Task<CommandResult> CreateCostCenterAsync(CostCenter costCenter);

        Task<CostCenter> GetCostCenterByIdAsync(int id);

        Task<CommandResult> UpdateCostCenterAsync(CostCenter costCenter);

        Task<IEnumerable<Section>> GetAllSectionAsync();

        Task<CommandResult> CreateSectionAsync(Section section);

        Task<Section> GetSectionByIdAsync(int id);

        Task<CommandResult> UpdateSectionAsync(Section section);

        Task<IEnumerable<SimpleItem>> GetAllJobCategoryAsync();

        Task<CommandResult> CreateJobCategoryAsync(SimpleItem item);

        Task<SimpleItem> GetJobCategoryByIdAsync(int id);

        Task<CommandResult> UpdateJobCategoryAsync(SimpleItem item);

        Task<IEnumerable<GradeLevel>> GetAllGradeLevelAsync();

        Task<CommandResult> CreateGradeLevelAsync(GradeLevel gradeLevel);

        Task<GradeLevel> GetGradeLevelByIdAsync(int id);

        Task<CommandResult> UpdateGradeLevelAsync(GradeLevel gradeLevel);

        Task<IEnumerable<JobGrade>> GetAllJobGradelAsync();

        Task<bool> IsExistedJobGradeAsync(JobGrade jobGrade);

        Task<CommandResult> CreateJobGradeAsync(JobGrade jobGrade);

        Task<JobGrade> GetJobGradeByIdAsync(int id);

        Task<CommandResult> UpdateJobGradeAsync(JobGrade jobGrade);

        Task<IEnumerable<SimpleItem>> GetAllCountryAsync();

        Task<bool> IsExistedCountryAsync(SimpleItem item);

        Task<CommandResult> CreateCountryAsync(SimpleItem item);

        Task<SimpleItem> GetCountryByIdAsync(int id);

        Task<CommandResult> UpdateCountryAsync(SimpleItem item);

        Task<IEnumerable<City>> GetAllCityAsync();

        Task<bool> IsExistedCityAsync(City city);

        Task<CommandResult> CreateCityAsync(City city);

        Task<City> GetCityByIdAsync(int id);

        Task<CommandResult> UpdateCityAsync(City city);

        Task<IEnumerable<Office>> GetAllOfficeAsync();

        Task<bool> IsExistedOfficeAsync(Office office);

        Task<CommandResult> CreateOfficeAsync(Office office);

        Task<Office> GetOfficeByIdAsync(int id);

        Task<CommandResult> UpdateOfficeAsync(Office office);

        Task<IEnumerable<SimpleItem>> GetAllRefResourceItemAsync(string refName);

        Task<bool> IsExistedRefResourceItemAsync(string refName, SimpleItem item);

        Task<CommandResult> CreateRefResourceItemAsync(string refName, SimpleItem item);

        Task<SimpleItem> GetRefResourceByNameAndIdAsync(string refName, int resourceId);

        Task<CommandResult> UpdateRefResourceItemAsync(string refName, SimpleItem item);

        Task<IEnumerable<SimpleItem>> GetAllCurrencyAsync();

        Task<bool> IsExistedCurrencyAsync(SimpleItem item);

        Task<CommandResult> CreateCurrencyAsync(SimpleItem item);

        Task<SimpleItem> GetCurrencyByIdAsync(int id);

        Task<CommandResult> UpdateCurrencyAsync(SimpleItem item);

        //// Common funcs
        Task<SelectList> GetDivisionSelectListByCompanyIdAsync(int companyId);

        Task<SelectList> GetDepartmentSelectListByDivisionIdAsync(int divisionId);

        Task<SelectList> GetCostCenterSelectListByDepartmentIdAsync(int departmentId);

        Task<SelectList> GetSectionSelectListByCostCenterIdAsync(int costCenterId);

        Task<SelectList> GetGradeLevelSelectListByJobCateogryIdAsync(int jobCategoryId);

        Task<SelectList> GetCitySelectListByCountryIdAsync(int countryId);

        Task<SelectList> GetCompanyBVISelectListAsync();

        Task<SelectList> GetJobCategorySelectListAsync();

        Task<SelectList> GetCountrySelectListAsync();
    }
}