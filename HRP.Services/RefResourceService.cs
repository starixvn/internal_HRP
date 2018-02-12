namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Models;

    public class RefResourceService : IRefResourceService
    {
        private readonly IBaseService baseService;

        public RefResourceService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IEnumerable<ReferenceResourceModel> GetReferenceResourcesByName(string resourceName)
        {
            var criteria = new GetRefResourceByNamesCriteria
            {
                RefNames = resourceName
            };

            var refResources = baseService.GetData<ReferenceResourceModel>(criteria);

            return refResources;
        }

        public IEnumerable<ReferenceResourceModel> GetReferenceResourcesByNames(string[] names)
        {
            IEnumerable<ReferenceResourceModel> refResources = new List<ReferenceResourceModel>();

            if (names != null)
            {
                var refNames = string.Join(",", names);
                var criteria = new GetRefResourceByNamesCriteria
                {
                    RefNames = refNames
                };

                refResources = baseService.GetData<ReferenceResourceModel>(criteria);
            }

            return refResources;
        }

        public IEnumerable<ReferenceResourceModel> FilterReferenceResources(
            IEnumerable<ReferenceResourceModel> refResources,
            string refName)
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