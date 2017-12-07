namespace HRP.Models
{
    public class TwoFactorAuthenticatorViewModel
    {
        public string OTPCode { get; set; }

        // TODO: Remove when 2 factor completed
        public bool IsByPassTwoFactor { get; set; }
    }
}