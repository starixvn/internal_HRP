namespace HRP.Services
{
    using Models;

    public interface IUserService
    {
        User GetUserByUsername(string username);

        void ResetUserPassword(string username);

        bool UpdateSecretKey(string username, string secretKey);

        string GenerateTokenOTPRegister(string username);

        bool ResetTokenOTPRegister(string username);

        void SendMailToRegisterOTP(string username, string tokenOTPRegister);

        bool UpdateOTPStatus(int userId, bool isOTPRegistered);
    }
}