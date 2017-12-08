namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TwoFactorAuthenticatorViewModel
    {
        [Required]
        public string OTPCode { get; set; }

        // TODO: Remove when 2 factor completed
        public bool IsByPassTwoFactor { get; set; }
    }
}