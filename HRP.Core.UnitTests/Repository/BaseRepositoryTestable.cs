namespace HRP.Core.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;

    public class BaseRepositoryTestable : BaseRepository
    {
        public IObjectDb Public_GetObjectDb(string key)
        {
            return GetObjectDb(key);
        }

        public CommandResult Public_ExecuteNonQuery(ICriteria criteria)
        {
             return ExecuteNonQuery(criteria);
        }

        public async Task<CommandResult> Public_ExecuteNonQueryAsync(ICriteria criteria)
        {
            return await ExecuteNonQueryAsync(criteria);
        }
    }
}