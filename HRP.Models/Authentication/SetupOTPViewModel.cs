namespace HRP.Models.Authentication
{
    public class SetupOtpViewModel
    {
        public string SecretKey { get; set; }

        public string QrCodeLink { get; set; }

        public string TextCode { get; set; }

        public string Otp { get; set; }

        public string Username { get; set; }
    }
}