namespace HRP.Services
{
    using HRP.Core.Model;
    using HRP.Criteria.User;
    using HRP.Models;

    public interface IUserService
    {
        User GetUserByUsername(string username);

        User GetUserByTokenResetPassword(string tokenResetPassword);

        CommandResult UpdateTokenResetPassword(string username, string token);

        void ResetUserPassword(string username);
    }
}