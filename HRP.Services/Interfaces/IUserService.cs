namespace HRP.Services
{
    using Models;

    public interface IUserService
    {
        User GetUserByUsername(string username);

        User GetUserByTokenOTPRegister(string token);

        void ResetUserPassword(string username);

        bool UpdateSecretKey(string username, string secretKey);

        string GenerateTokenOTPRegister(string username);

        bool ResetTokenOTPRegister(string username);

        void SendMailToRegisterOTP(string tokenOTPRegister);

        bool UpdateOTPStatus(int userId, bool isOTPRegistered);

        bool IsCorrectPassword(string password, string hashedPassword);
    }
}