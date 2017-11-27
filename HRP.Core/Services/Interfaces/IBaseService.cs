namespace HRP.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Model;

    public interface IBaseService
    {
        IEnumerable<T> GetData<T>(ICriteria criteria);

        Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria);

        T GetDataEntity<T>(ICriteria criteria);

        Task<T> GetDataEntityAsync<T>(ICriteria criteria);

        CommandResult ExecuteNonQuery(ICriteria criteria);

        Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria);
    }
}