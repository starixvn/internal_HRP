namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public class TwoFactorAuthenticatorViewModel
    {
        [Required]
        [Display(Name = nameof(ViewLang.OtpCode), ResourceType = typeof(ViewLang))]
        public string OTPCode { get; set; }

        public string RijndaelKey { get; set; }
        public string RijndaelIV { get; set; }

        // TODO: Remove when 2 factor completed
        public bool IsByPassTwoFactor { get; set; }
    }
}