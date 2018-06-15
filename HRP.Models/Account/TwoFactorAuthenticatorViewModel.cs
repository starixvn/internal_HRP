namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;
    using SystemSecurity;
    using Resources;

    public class TwoFactorAuthenticatorViewModel : RijndaelData
    {
        [Required]
        [Display(Name = nameof(ViewLang.OtpCode), ResourceType = typeof(ViewLang))]
        public string OTPCode { get; set; }

        // TODO: Remove when 2 factor completed
        public bool IsByPassTwoFactor { get; set; }
    }
}