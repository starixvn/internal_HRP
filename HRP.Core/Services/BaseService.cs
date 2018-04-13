namespace HRP.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Model;
    using Repositories;

    public class BaseService : IBaseService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IBaseRepository baseRepository;

        public BaseService(
            IDynamicRepository dynamicRepository = null,
            IBaseRepository baseRepository = null)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            this.baseRepository = baseRepository ?? new BaseRepository();
        }

        public virtual async Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.FetchAsync<T>(criteria).ConfigureAwait(false);
            }

            return new List<T>();
        }

        public virtual async Task<T> GetDataEntityAsync<T>(ICriteria criteria)
        {
            if (criteria != null && criteria.IsValid())
            {
                return await dynamicRepository.GetAsync<T>(criteria).ConfigureAwait(false);
            }

            return default(T);
        }

        public virtual async Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = await baseRepository.ExecuteNonQueryAsync(criteria).ConfigureAwait(false);
            }

            return commandResult;
        }
    }
}