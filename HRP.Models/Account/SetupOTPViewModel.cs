namespace HRP.Models.Account
{
    using System.ComponentModel.DataAnnotations;

    public class SetupOtpViewModel
    {
        public string TextCode { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "OTP code must be 6 digits")]
        [Display(Name = "OTP code")]
        [DataType(DataType.Password)]
        public string Otp { get; set; }

        public string Username { get; set; }

        public string SystemName { get; set; }

        public string QrCodeText => $"otpauth://totp/{SystemName}:{Username}?secret={TextCode}&issuer={SystemName}";
    }
}