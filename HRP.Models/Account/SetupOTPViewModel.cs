namespace HRP.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using SystemSecurity;
    using Resources;

    public class SetupOtpViewModel : RijndaelData
    {
        public string TextCode { get; set; }

        [Required]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessageResourceName = nameof(Message.OtpLimit), ErrorMessageResourceType = typeof(Message))]
        [Display(Name = nameof(ViewLang.OtpCode), ResourceType = typeof(ViewLang))]
        public string Otp { get; set; }

        public string Username { get; set; }

        public string AppName { get; set; }

        public string QrCodeText => $"otpauth://totp/{AppName}:{Username}?secret={TextCode}&issuer={AppName}";
    }
}