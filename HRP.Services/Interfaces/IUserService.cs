namespace HRP.Services
{
    using System.Threading.Tasks;
    using Models;

    public interface IUserService
    {
        Task<User> GetUserByUsernameAsync(string username);

        Task<User> GetUserByTokenOTPRegisterAsync(string token);

        Task ResetUserPasswordAsync(string username);

        Task<bool> UpdateSecretKeyAsync(string username, string secretKey);

        Task<string> GenerateTokenOTPRegisterAsync(string username);

        Task<bool> ResetTokenOTPRegisterAsync(string username);

        Task SendMailToRegisterOTPAsync(string tokenOTPRegister);

        Task<bool> UpdateOTPStatusAsync(int userId, bool isOTPRegistered);

        bool IsCorrectPassword(string password, string hashedPassword);
    }
}