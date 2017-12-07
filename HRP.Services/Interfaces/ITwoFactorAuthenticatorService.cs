namespace HRP.Services
{
    public interface ITwoFactorAuthenticatorService
    {
        string GenerateQrImageUrl(string username, string secretKey);

        string GenerateSetupCode(string username, string secretKey);

        bool ValidateOTPCode(string username, string secretKey, int otpCode);
    }
}