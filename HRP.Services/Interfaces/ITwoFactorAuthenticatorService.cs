namespace HRP.Services
{
    public interface ITwoFactorAuthenticatorService
    {
         string GenerateQrImageUrl(string username, string secretkey);

         string GenerateSetupCode(string username, string secretkey);

         bool ValidateOTPCode(string username, string secretkey, string otpCode);
    }
}