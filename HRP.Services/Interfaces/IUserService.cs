namespace HRP.Services
{
    using HRP.Core.Model;
    using HRP.Criteria;
    using HRP.Models;

    public interface IUserService
    {
        User GetUserByUsername(string username);

        CommandResult UpdateTokenResetPassword(string username, string token);

        void ResetUserPassword(string username);
    }
}