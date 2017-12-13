namespace HRP.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class ChangePasswordViewModel : ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PasswordRequired")]
        public string OldPassword { get; set; }
    }
}