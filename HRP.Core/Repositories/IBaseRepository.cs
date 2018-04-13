namespace HRP.Core.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Model;

    public interface IBaseRepository
    {
        Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria);
    }
}