namespace HRP.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.DataConfiguration;

    public interface IDataConfigurationService
    {
        Task<IEnumerable<SimpleItem>> GetAllCompanyBVIAsync();

        Task<CommandResult> CreateCompanyBVIAsync(SimpleItem item);
    }
}