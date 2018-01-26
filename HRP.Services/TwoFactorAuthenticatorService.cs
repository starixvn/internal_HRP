namespace HRP.Services
{
    using System;
    using Core.Model;
    using Google.Authenticator;
    using Interfaces;
    using Microsoft.Extensions.Options;

    public class TwoFactorAuthenticatorService : ITwoFactorAuthenticatorService
    {
        private readonly int qrImageWidth = 200;
        private readonly int qrImageHeight = 200;

        private readonly IOptionsSnapshot<OtpConfig> config;

        public TwoFactorAuthenticatorService(IOptionsSnapshot<OtpConfig> config)
        {
            this.config = config;
        }

        public string SetupGoogleAuthInfo(string username, string secretKey)
        {
            var twoFactorAuthen = new TwoFactorAuthenticator();
            var setupInfo = twoFactorAuthen.GenerateSetupCode(config.Value.AppName, username, secretKey, qrImageWidth, qrImageHeight);
            return setupInfo.ManualEntryKey;
        }

        public bool ValidateOTPCode(string secretKey, string otpCode)
        {
            var timeExpireOTP = TimeSpan.FromSeconds(config.Value.OtpExpireTime);

            var twoFactorAuthenticator = new TwoFactorAuthenticator();

            // document: http://brandonpotter.com/2014/09/07/implementing-free-two-factor-authentication-in-net-using-google-authenticator/
            // accept +/- 5 minute tolerate

            var validOTP = twoFactorAuthenticator.ValidateTwoFactorPIN(secretKey, otpCode, timeExpireOTP);

            return validOTP;
        }
    }
}