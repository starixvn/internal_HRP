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

        SelectList GetSelectList(IEnumerable<ReferenceResourceModel> refResources);
    }
}