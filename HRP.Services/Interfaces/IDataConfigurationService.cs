namespace HRP.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models;
    using HRP.Models.DataConfiguration;

    public interface IDataConfigurationService
    {
        IEnumerable<Field> GetDataForIndexPage();

        Task<IEnumerable<SimpleItem>> GetAllCompanyBVIAsync();

        Task<bool> IsExistedCompanyBVIAsync(SimpleItem item);

        Task<SimpleItem> GetCompanyBVIByIdAsync(int id);

        Task<CommandResult> CreateCompanyBVIAsync(SimpleItem item);

        Task<CommandResult> UpdateCompanyBVIAsync(SimpleItem item);

        Task<DivisionTableViewModel> GetAllDivisionAsync();
    }
}