namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;

    public class TwoFactorAuthenticatorViewModel
    {
        [Required]
        [MaxLength(6)]
        public string VerifyCode { get; set; }
    }
}