namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TwoFactorAuthenticatorViewModel
    {
        [Required]
        [Display(Name = "OTP code")]
        public string OTPCode { get; set; }

        // TODO: Remove when 2 factor completed
        public bool IsByPassTwoFactor { get; set; }
    }
}