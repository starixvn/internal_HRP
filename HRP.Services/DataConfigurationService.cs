namespace HRP.Services
{
    using System;
    using System.Threading.Tasks;
    using HRP.Core.Enumerations;
    using HRP.Core.Model;
    using HRP.Core.Services;
    using HRP.Criteria.Company;
    using HRP.Models.DataConfiguration;
    using HRP.Services.Interfaces;

    public class DataConfigurationService : IDataConfigurationService
    {
        private readonly IBaseService baseService;

        public DataConfigurationService(IBaseService baseService)
        {
            this.baseService = baseService;
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

        public DataConfigurationIndexViewModel GetActionLinkForManageItem()
        {
            throw new NotImplementedException();
        }
    }
}