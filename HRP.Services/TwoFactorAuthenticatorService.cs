namespace HRP.Services
{
    using Google.Authenticator;
    using System;

    public class TwoFactorAuthenticatorService : ITwoFactorAuthenticatorService
    {
        private readonly string _appName = "HRP";
        private readonly int qrImageWidth = 200;
        private readonly int qrImageHeight = 200;
        private readonly int otpExpireTime = 30;

        private SetupCode SetupGoogleAuthInfo(string username, string secretKey)
        {
            var twoFactorAuthen = new TwoFactorAuthenticator();
            var uniqueKey = username + secretKey;
            var setupInfo = twoFactorAuthen.GenerateSetupCode(_appName, username, uniqueKey, qrImageWidth, qrImageHeight);
            return setupInfo;
        }

        public string GenerateQrImageUrl(string username, string secretKey)
        {
            var setupInfo = SetupGoogleAuthInfo(username, secretKey);
            return setupInfo.QrCodeSetupImageUrl;
        }

        public string GenerateSetupCode(string username, string secretKey)
        {
            var setupInfo = SetupGoogleAuthInfo(username, secretKey);
            return setupInfo.ManualEntryKey;
        }

        public bool ValidateOTPCode(string username, string secretKey, int otpCode)
        {
            var validOTP = false;
            var twoFactorAuthen = new TwoFactorAuthenticator();
            var uniqueKey = username + secretKey;
            var timeExpireOTP = TimeSpan.FromSeconds(otpExpireTime);
            twoFactorAuthen.ValidateTwoFactorPIN(uniqueKey, otpCode.ToString(), timeExpireOTP);

            return validOTP;
        }
    }
}