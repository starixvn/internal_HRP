namespace HRP.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Model;

    public interface IBaseService
    {
        Task<IEnumerable<T>> GetDataAsync<T>(ICriteria criteria);

        Task<T> GetDataEntityAsync<T>(ICriteria criteria);

        Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>>> FetchMultipleAsync<TFirst, TSecond>(ICriteria criteria);

        Task<Tuple<IEnumerable<TFirst>, IEnumerable<TSecond>, IEnumerable<TThird>>> FetchMultipleAsync<TFirst, TSecond, TThird>(ICriteria criteria);

        Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria);
    }
}