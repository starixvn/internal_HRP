namespace HRP.Services.Interfaces
{
    using System.Collections.Generic;
    using Models;

    public interface IRefResourceService
    {
        IEnumerable<ReferenceResourceModel> GetReferenceResourcesByNames(string[] names);

        IEnumerable<ReferenceResourceModel> FilterReferenceResources(
            IEnumerable<ReferenceResourceModel> refResources,
            string resourceName);
    }
}