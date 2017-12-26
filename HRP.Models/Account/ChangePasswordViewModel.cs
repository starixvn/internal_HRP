namespace HRP.Models.Account
{
    using HRP.Resources;
    using System.ComponentModel.DataAnnotations;

    public class ChangePasswordViewModel : ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PasswordRequired")]
        public string OldPassword { get; set; }
    }
}