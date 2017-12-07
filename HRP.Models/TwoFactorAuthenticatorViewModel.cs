namespace HRP.Models
{
    public class TwoFactorAuthenticatorViewModel
    {
        public string OTPCode { get; set; }

        public bool SkipTwoFactor { get; set; }
    }
}