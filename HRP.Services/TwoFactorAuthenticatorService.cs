namespace HRP.Services
{
    using System;
    using System.Diagnostics;
    using Google.Authenticator;

    public class TwoFactorAuthenticatorService : ITwoFactorAuthenticatorService
    {
        private readonly string _appName = "HRP";
        private readonly int _qrImageWidth = 200;
        private readonly int _qrImageHeight = 200;
        private readonly int _otpExpireTime = 0;

        public SetupCode SetupGoogleAuthInfo(string username, string secretKey)
        {
            var twoFactorAuthen = new TwoFactorAuthenticator();
            var setupInfo = twoFactorAuthen.GenerateSetupCode(_appName, username, secretKey, _qrImageWidth, _qrImageHeight);
            return setupInfo;
        }

        public bool ValidateOTPCode(string secretKey, string otpCode)
        {
            var timeExpireOTP = TimeSpan.FromSeconds(_otpExpireTime);

            var twoFactorAuthenticator = new TwoFactorAuthenticator();

            // document: http://brandonpotter.com/2014/09/07/implementing-free-two-factor-authentication-in-net-using-google-authenticator/
            // accept +/- 5 minute tolerate

            var validOTP = twoFactorAuthenticator.ValidateTwoFactorPIN(secretKey, otpCode, timeExpireOTP);
            Debug.WriteLine("OTP: " + otpCode);
            Debug.WriteLine("Valid codes" + twoFactorAuthenticator.GetCurrentPINs(secretKey, timeExpireOTP)[0]);

            return validOTP;
        }
    }
}