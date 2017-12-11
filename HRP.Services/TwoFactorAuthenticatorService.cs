namespace HRP.Services
{
    using System;
    using Google.Authenticator;

    public class TwoFactorAuthenticatorService : ITwoFactorAuthenticatorService
    {
        private readonly string AppName = "HRP";
        private readonly int qrImageWidth = 200;
        private readonly int qrImageHeight = 200;
        private readonly int otpExpireTime = 0;

        public SetupCode SetupGoogleAuthInfo(string username, string secretKey)
        {
            var twoFactorAuthen = new TwoFactorAuthenticator();
            var uniqueKey = username + secretKey;
            var setupInfo = twoFactorAuthen.GenerateSetupCode(AppName, username, uniqueKey, qrImageWidth, qrImageHeight);
            return setupInfo;
        }

        public bool ValidateOTPCode(string username, string secretKey, string otpCode)
        {
            var uniqueKey = username + secretKey;
            var timeExpireOTP = TimeSpan.FromSeconds(otpExpireTime);

            var twoFactorAuthenticator = new TwoFactorAuthenticator();
            twoFactorAuthenticator.ValidateTwoFactorPIN(uniqueKey, otpCode, timeExpireOTP);

            // document: http://brandonpotter.com/2014/09/07/implementing-free-two-factor-authentication-in-net-using-google-authenticator/
            // accept +/- 5 minute tolerate
            var validOTP = twoFactorAuthenticator.ValidateTwoFactorPIN(uniqueKey, otpCode);

            return validOTP;
        }
    }
}