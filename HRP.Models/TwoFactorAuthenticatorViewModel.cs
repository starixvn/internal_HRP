namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TwoFactorAuthenticatorViewModel
    {
        [Required]
        [MaxLength(6)]
        public string OTPCode { get; set; }

        public bool SkipTwoFactor { get; set; }
    }
}