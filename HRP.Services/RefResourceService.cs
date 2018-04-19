namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services;
    using HRP.Core.Criteria;
    using HRP.Core.Model;

    public class RefResourceService : IRefResourceService
    {
        private readonly IBaseService baseService;

        public RefResourceService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<ReferenceResourceModel>> GetReferenceResourcesByNamesAsync(string[] names, bool? isActive = true)
        {
            IEnumerable<ReferenceResourceModel> refResources = new List<ReferenceResourceModel>();

            if (names != null)
            {
                var refNames = string.Join(",", names);
                var criteria = new GetRefResourceByNamesCriteria
                {
                    RefNames = refNames,
                    IsActive = isActive
                };

                refResources = await baseService.GetDataAsync<ReferenceResourceModel>(criteria).ConfigureAwait(false);
            }

            return refResources;
        }

        public IEnumerable<ReferenceResourceModel> FilterReferenceResources(
            IEnumerable<ReferenceResourceModel> refResources,
            string refName,
            bool? isActive = true)
        {
            IEnumerable<ReferenceResourceModel> referenceResources = new List<ReferenceResourceModel>();

            if (refResources != null && !string.IsNullOrWhiteSpace(refName))
            {
                referenceResources = refResources.Where(x => x.RefName == refName);
            }

            return referenceResources;
        }
    }
}