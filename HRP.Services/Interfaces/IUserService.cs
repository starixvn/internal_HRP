namespace HRP.Services
{
    using HRP.Models;

    public interface IUserService
    {
        User GetUserByUsername(string username);

        void ResetUserPassword(string username);

        void OTPRegister(string username);
    }
}