namespace HRP.Services
{
    using HRP.Core.Model;
    using HRP.Criteria;
    using HRP.Models;

    public interface IUserService
    {
        User GetUserByUsername(string username);

        GetUsersResult GetUsers(string keyword, bool? isActive, int pageNumber, int pageSize);

        CommandResult UpdateTokenResetPassword(string username, string token);

        void ResetUserPassword(string username);
    }
}