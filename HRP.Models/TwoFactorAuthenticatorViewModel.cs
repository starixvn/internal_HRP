namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TwoFactorAuthenticatorViewModel
    {
        [Required]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "OTP code must be 6 digits")]
        [Display(Name = "OTP code")]
        [DataType(DataType.Password)]
        public string OTPCode { get; set; }

        // TODO: Remove when 2 factor completed
        public bool IsByPassTwoFactor { get; set; }
    }
}