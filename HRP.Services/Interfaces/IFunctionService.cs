namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.UserPermission;

    public interface IFunctionService
    {
        Task<IEnumerable<FunctionModel>> GetTreeFunctions(bool? isActive);
    }
}