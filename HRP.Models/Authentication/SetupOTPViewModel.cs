namespace HRP.Models.Authentication
{
    public class SetupOtpViewModel
    {
        public string QrCodeLink { get; set; }

        public string TextCode { get; set; }

        public int Otp { get; set; }
    }
}
