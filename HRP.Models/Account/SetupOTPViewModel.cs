namespace HRP.Models.Account
{
    public class SetupOtpViewModel
    {
        ////public string SecretKey { get; set; }

        ////public string QrCodeLink { get; set; }

        public string TextCode { get; set; }

        public string Otp { get; set; }

        public string Username { get; set; }

        public string SystemName { get; set; }

        public string QrCodeText => $"otpauth://totp/{SystemName}:{Username}?secret={TextCode}&issuer={SystemName}";
    }
}