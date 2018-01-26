namespace HRP.Services.Interfaces
{
    public interface ITwoFactorAuthenticatorService
    {
        string SetupGoogleAuthInfo(string username, string secretKey);

        bool ValidateOTPCode(string secretKey, string otpCode);
    }
}