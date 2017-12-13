namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.UserPermisson;

    public interface IFunctionService
    {
        Task<IEnumerable<FunctionModel>> GetFunctions(bool? isActive);

        Task<IEnumerable<FunctionModel>> GetTreeFunctions(bool? isActive);
    }
}