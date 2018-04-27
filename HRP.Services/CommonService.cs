namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Criteria;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Models;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CommonService : ICommonService
    {
        private readonly IBaseService baseService;
        private readonly ISessionService sessionService;

        public CommonService(
            IBaseService baseService,
            ISessionService sessionService)
        {
            this.baseService = baseService;
            this.sessionService = sessionService;
        }

        public async Task<IEnumerable<ReferenceResourceModel>> GetReferenceResourcesByNamesAsync(string[] refNames, bool? isActive = true)
        {
            IEnumerable<ReferenceResourceModel> refResources = new List<ReferenceResourceModel>();

            if (refNames != null)
            {
                var criteria = new GetRefResourceByNamesCriteria
                {
                    RefNames = string.Join(",", refNames),
                    IsActive = isActive
                };

                refResources = await baseService.GetDataAsync<ReferenceResourceModel>(criteria).ConfigureAwait(false);
            }

            return refResources;
        }

        public IEnumerable<ReferenceResourceModel> FilterReferenceResources(IEnumerable<ReferenceResourceModel> refResources, string refName)
        {
            IEnumerable<ReferenceResourceModel> referenceResources = new List<ReferenceResourceModel>();

            if (refResources != null && !string.IsNullOrWhiteSpace(refName))
            {
                referenceResources = refResources.Where(x => x.RefName == refName);
            }

            return referenceResources;
        }

        public async Task<IEnumerable<Company>> GetCompanyByFunctionKeyAsync(string functionKey)
        {
            var criteria = new GetCompanyByFunctionKeyCriteria
            {
                UserId = sessionService.UserId,
                FunctionKey = functionKey
            };

            return await baseService.GetDataAsync<Company>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<JobCategory>> GetAllJobCategoryAsync(bool? isActive = true)
        {
            var criteria = new GetAllJobCategoryCriteria { IsActive = isActive };

            return await baseService.GetDataAsync<JobCategory>(criteria).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Country>> GetAllCountryAsync(bool? isActive = true)
        {
            var criteria = new GetAllCountryCriteria { IsActive = isActive };

            return await baseService.GetDataAsync<Country>(criteria).ConfigureAwait(false);
        }

        public SelectList GetSelectList<T>(IEnumerable<T> items, string dataValueField, string dataTextField)
        {
            return new SelectList(items, dataValueField, dataTextField);
        }
    }
}