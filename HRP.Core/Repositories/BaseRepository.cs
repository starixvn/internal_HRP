namespace HRP.Core.Repositories
{
    using System;
    using System.Collections;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Model;

    public class BaseRepository : IBaseRepository
    {
        public virtual CommandResult ExecuteNonQuery(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();
                hash.Add("ErrCode", 0);
                hash.Add("ErrMsg", string.Empty);

                db.ExecuteNonQuery(hash);
                CommandResult commandResult = GetCommandResult(hash);

                return commandResult;
            }
        }

        public virtual async Task<CommandResult> ExecuteNonQueryAsync(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();
                hash.Add("ErrCode", 0);
                hash.Add("ErrMsg", string.Empty);

                await db.ExecuteNonQueryAsync(hash).ConfigureAwait(false);
                CommandResult commandResult = GetCommandResult(hash);

                return commandResult;
            }
        }

        private CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errcode"];
            var errMsg = hash["errmsg"];

            return new CommandResult
            {
                Code = (errCode is DBNull) ? 0 : Convert.ToInt32(errCode),
                Message = (errMsg is DBNull) ? string.Empty : Convert.ToString(errMsg)
            };
        }
    }
}