namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria;
    using HRP.Criteria.Company;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Services.Interfaces;

    public class DataConfigurationService : IDataConfigurationService
    {
        private readonly IBaseService baseService;

        public DataConfigurationService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<SimpleItem>> GetAllCompanyBVIAsync()
        {
            var companies = await baseService.GetDataAsync<Company>(new GetAllCompanyCriteria()).ConfigureAwait(false);

            if (companies != null)
            {
                var result = companies.Select(c => new SimpleItem
                {
                    ItemName = c.Code,
                    Description = c.Name,
                    IsActive = c.IsActive
                });

                return result;
            }

            return null;
        }

        public async Task<CommandResult> CreateCompanyBVIAsync(SimpleItem item)
        {
            var criteria = new CreateCompanyCriteria
            {
                Code = item.ItemName,
                Name = item.Description,
                IsActive = item.IsActive
            };

            var commandResult = await baseService.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);

            return commandResult;
        }
    }
}