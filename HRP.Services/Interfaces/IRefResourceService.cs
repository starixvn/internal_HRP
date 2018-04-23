namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;

    public interface IRefResourceService
    {
        Task<IEnumerable<ReferenceResourceModel>> GetReferenceResourcesByNamesAsync(string[] names, bool? isActive = true);

        IEnumerable<ReferenceResourceModel> FilterReferenceResources(
            IEnumerable<ReferenceResourceModel> refResources,
            string refName);
    }
}