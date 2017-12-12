namespace HRP.Models.Account
{
    using Core.Enumerations;
    using UserProfile;

    public class AccountViewModel
    {
        public AccountTab ActivePanel { get; set; }

        public bool IsOTPRegistered { get; set; }

        public SetupOtpViewModel SetupOtpViewModel { get; set; }

        public ChangePasswordViewModel ChangePasswordViewModel { get; set; }
    }
}