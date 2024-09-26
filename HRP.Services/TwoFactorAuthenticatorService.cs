namespace HRP.Services
{
    using System;
    using Core.Model;
    using Google.Authenticator;

    public class TwoFactorAuthenticatorService(OtpConfig config) : ITwoFactorAuthenticatorService
    {
        public string SetupGoogleAuthInfo(string username, string secretKey)
        {
            var twoFactorAuthen = new TwoFactorAuthenticator();
            var setupInfo = twoFactorAuthen.GenerateSetupCode(config.AppName, username, secretKey, false);
            return setupInfo.ManualEntryKey;
        }

        public bool ValidateOTPCode(string secretKey, string otpCode)
        {
            var timeExpireOTP = TimeSpan.FromSeconds(config.OtpExpireTime);

            var twoFactorAuthenticator = new TwoFactorAuthenticator();

            // document: http://brandonpotter.com/2014/09/07/implementing-free-two-factor-authentication-in-net-using-google-authenticator/
            // accept +/- 5 minute tolerate

            var validOTP = twoFactorAuthenticator.ValidateTwoFactorPIN(secretKey, otpCode, timeExpireOTP);

            return validOTP;
        }
    }
}