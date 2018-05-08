namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public interface ICommonService
    {
        Task<IEnumerable<ReferenceResourceModel>> GetReferenceResourcesByNamesAsync(string[] refNames, bool? isActive = true);

        IEnumerable<ReferenceResourceModel> FilterReferenceResources(IEnumerable<ReferenceResourceModel> refResources, string refName);

        Task<IEnumerable<Company>> GetCompanyByFunctionKeyAsync(string functionKey);

        Task<IEnumerable<Division>> GetDivisionByCompanyIdAsync(int companyId, bool? isActive = true);

        Task<IEnumerable<JobCategory>> GetAllJobCategoryAsync(bool? isActive = true);

        Task<IEnumerable<Country>> GetAllCountryAsync(bool? isActive = true);

        SelectList GetSelectList<T>(IEnumerable<T> items, string dataValueField, string dataTextField);
    }
}