namespace HRP.Core.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using HRP.Core.Model;

    public interface IBaseRepository
    {
        Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command);
    }
}