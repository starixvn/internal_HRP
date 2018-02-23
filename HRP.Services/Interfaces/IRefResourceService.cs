namespace HRP.Services.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface IRefResourceService
    {
        IEnumerable<ReferenceResourceModel> GetReferenceResourcesByName(string resourceName, bool? isActive = true);

        IEnumerable<ReferenceResourceModel> GetReferenceResourcesByNames(string[] names, bool? isActive = true);

        IEnumerable<ReferenceResourceModel> FilterReferenceResources(
            IEnumerable<ReferenceResourceModel> refResources,
            string refName,
            bool? isActive = true);
    }
}