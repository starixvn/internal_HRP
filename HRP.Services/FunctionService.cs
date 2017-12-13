namespace HRP.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Services;
    using HRP.Criteria;
    using Models.UserPermisson;

    public class FunctionService : IFunctionService
    {
        private readonly IBaseService baseService;

        public FunctionService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<IEnumerable<FunctionModel>> GetFunctions(bool? isActive)
        {
            var criteria = new GetAllFunctionCriteria
            {
                IsActive = isActive
            };

            return await baseService.GetDataAsync<FunctionModel>(criteria).ConfigureAwait(false);
        }

        public IEnumerable<FunctionModel> BuildTreeFunction(IEnumerable<FunctionModel> functions)
        {
            IEnumerable<FunctionModel> parentFunctions;

            if (functions != null && functions.Any())
            {
                parentFunctions = functions.Where(x => !x.ParentId.HasValue);

                foreach (var function in parentFunctions)
                {
                    function.Level = 0;
                    GetChildFunctions(function, functions);
                }
            }
            else
            {
                parentFunctions = new List<FunctionModel>();
            }

            return parentFunctions;
        }

        private void GetChildFunctions(FunctionModel function, IEnumerable<FunctionModel> functions)
        {
            IEnumerable<FunctionModel> functionItems = functions.Where(x => x.ParentId == function.FunctionId);

            if (functionItems != null && functionItems.Any())
            {
                foreach (var item in functionItems)
                {
                    item.Level = function.Level + 1;
                    function.Childs.Add(item);
                    GetChildFunctions(item, functions);
                }
            }
        }
    }
}