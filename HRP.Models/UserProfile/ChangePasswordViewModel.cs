namespace HRP.Models.UserProfile
{
    using System.ComponentModel.DataAnnotations;

    public class ChangePasswordViewModel : ResetPasswordViewModel
    {
        [Required]
        public string OldPassword { get; set; }
    }
}