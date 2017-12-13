namespace HRP.Services
{
    using System;
    using System.Diagnostics;
    using Core.Model;
    using Google.Authenticator;
    using Microsoft.Extensions.Options;

    public class TwoFactorAuthenticatorService : ITwoFactorAuthenticatorService
    {
        private readonly int _qrImageWidth = 200;
        private readonly int _qrImageHeight = 200;

        private readonly IOptionsSnapshot<OtpConfig> _config;

        public TwoFactorAuthenticatorService() : this(null)
        {
        }

        public TwoFactorAuthenticatorService(IOptionsSnapshot<OtpConfig> config)
        {
            _config = config;
        }

        public string SetupGoogleAuthInfo(string username, string secretKey)
        {
            var twoFactorAuthen = new TwoFactorAuthenticator();
            var setupInfo = twoFactorAuthen.GenerateSetupCode(_config.Value.AppName, username, secretKey, _qrImageWidth, _qrImageHeight);
            return setupInfo.ManualEntryKey;
        }

        public bool ValidateOTPCode(string secretKey, string otpCode)
        {
            var timeExpireOTP = TimeSpan.FromSeconds(_config.Value.OtpExpireTime);

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