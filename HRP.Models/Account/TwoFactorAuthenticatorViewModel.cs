namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Models.SystemSecurity;
    using HRP.Resources;

    public class TwoFactorAuthenticatorViewModel : RijndaelData
    {
        [Required]
        [Display(Name = nameof(ViewLang.OtpCode), ResourceType = typeof(ViewLang))]
        public string OTPCode { get; set; }
    }
}