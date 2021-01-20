namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models;
    using HRP.Models.BankAccount;
    using HRP.Models.Currency;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICommonService
    {
        Task<IEnumerable<ReferenceResourceModel>> GetReferenceResourcesByNamesAsync(string[] refNames, bool? isActive = true);

        IEnumerable<ReferenceResourceModel> FilterReferenceResources(IEnumerable<ReferenceResourceModel> refResources, string refName);

        Task<IEnumerable<Company>> GetCompanyByFunctionKeyAsync(string functionKey);

        Task<IEnumerable<Division>> GetDivisionByCompanyIdAsync(int companyId, bool? isActive = true);

        Task<IEnumerable<Department>> GetDepartmentByDivisionIdAsync(int divisionId, bool? isActive = true);

        Task<IEnumerable<CostCenter>> GetCostCenterByDepartmentIdAsync(int departmentId, bool? isActive = true);

        Task<IEnumerable<Section>> GetSectionByCostCenterIdAsync(int costCenterId, bool? isActive = true);

        Task<IEnumerable<GradeLevel>> GetGradeLevelByJobCategoryIdAsync(int jobCategoryId, bool? isActive = true);

        Task<IEnumerable<JobGrade>> GetJobGradeByGradeLevelIdAsync(int gradeLevelId, bool? isActive = true);

        Task<IEnumerable<JobCategory>> GetAllJobCategoryAsync(bool? isActive = true);

        Task<IEnumerable<BankAccount>> GetEmployeeBankAccountAsync(int employeeId, int bankTypeId, bool? isActive = true);

        SelectList GetSelectList<T>(IEnumerable<T> items, string dataValueField, string dataTextField);

        Task<IEnumerable<Currency>> GetCurrencyAsync(bool? isActive = true);
    }
}