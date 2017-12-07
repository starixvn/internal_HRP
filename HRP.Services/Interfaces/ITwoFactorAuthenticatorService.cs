namespace HRP.Services
{
    using Google.Authenticator;

    public interface ITwoFactorAuthenticatorService
    {
        SetupCode SetupGoogleAuthInfo(string username, string secretKey);

        bool ValidateOTPCode(string username, string secretKey, int otpCode);
    }
}