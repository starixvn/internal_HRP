namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Models;

    public class RefResourceService : BaseService, IRefResourceService
    {
        private readonly IBaseService baseService;

        public RefResourceService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public IEnumerable<ReferenceResourceModel> GetReferenceResourcesByNames(string[] names)
        {
            var refNames = string.Join(",", names);

            return baseService.GetData<ReferenceResourceModel>(
                new GetRefResourceByNamesCriteria
                {
                    RefNames = refNames
                });
        }

        public IEnumerable<ReferenceResourceModel> FilterReferenceResources(
            IEnumerable<ReferenceResourceModel> refResources,
            string resourceName)
        {
            return refResources.Where(x => x.RefName == resourceName);
        }
    }
}