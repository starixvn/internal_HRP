namespace HRP.Services.Interfaces
{
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models.DataConfiguration;

    public interface IDataConfigurationService
    {
        Task<CommandResult> CreateCompanyBVIAsync(SimpleItem item);

        DataConfigurationIndexViewModel GetActionLinkForManageItem();
    }
}