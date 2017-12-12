namespace HRP.Models.Account
{
    using UserProfile;

    public class AccountViewModel
    {
        public string ActivePanel { get; set; }

        public bool IsOTPRegistered { get; set; }

        public SetupOtpViewModel SetupOtpViewModel { get; set; }

        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }
    }
}