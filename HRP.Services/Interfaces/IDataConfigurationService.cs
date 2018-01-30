namespace HRP.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using HRP.Core.Model;
    using HRP.Models;
    using HRP.Models.DataConfiguration;
    using HRP.Models.DataConfiguration.Department;

    public interface IDataConfigurationService
    {
        IEnumerable<Field> GetDataForIndexPage();

        Task<IEnumerable<SimpleItem>> GetAllCompanyBVIAsync();

        Task<bool> IsExistedCompanyBVIAsync(SimpleItem item);

        Task<SimpleItem> GetCompanyBVIByIdAsync(int id);

        Task<CommandResult> CreateCompanyBVIAsync(SimpleItem item);

        Task<CommandResult> UpdateCompanyBVIAsync(SimpleItem item);

        Task<IEnumerable<Division>> GetAllDivisionAsync();

        Task<CreateDivisionViewModel> GetDataForCreateDivisionAsync();

        Task<bool> IsExistedDivisionAsync(Division division);

        Task<CommandResult> CreateDivisionAsync(Division division);

        Task<Division> GetDivisionByIdAsync(int id);

        Task<CommandResult> UpdateDivisionAsync(Division division);

        Task<IEnumerable<Department>> GetAllDepartmentAsync();

        Task<CreateDepartmentViewModel> GetDataForCreateDepartmentAsync();
    }
}