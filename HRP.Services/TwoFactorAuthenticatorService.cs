namespace HRP.Services
{
    using HRP.Services.Interfaces;
    using System;
    using Google.Authenticator;

    public class TwoFactorAuthenticatorService : ITwoFactorAuthenticatorService
    {
        private string _appName = "HRP";
        private int qrImageWidth = 200;
        private int qrImageHeight = 200;

        private int otpExpireTime = 30;

        private SetupCode SetupGoogleAuthInfo(string username, string secretKey)
        {
            TwoFactorAuthenticator twoFactorAuthen = new TwoFactorAuthenticator();
            string UniqueKey = username + secretKey;
            var setupInfo = twoFactorAuthen.GenerateSetupCode(_appName, username, UniqueKey, qrImageWidth, qrImageHeight);
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

        public bool ValidateOTPCode(string username, string secretkey, string otpCode)
        {
            bool validOTP = false;
            TwoFactorAuthenticator twoFactorAuthen = new TwoFactorAuthenticator();
            string uniqueKey = username + secretkey;
            TimeSpan timeExpireOTP = TimeSpan.FromSeconds(otpExpireTime);
            twoFactorAuthen.ValidateTwoFactorPIN(uniqueKey, otpCode, timeExpireOTP);
            return validOTP;
        }
    }
}