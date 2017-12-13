namespace HRP.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PasswordRequired")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,50}$", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PasswordContainRequired")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PasswordNotMatch")]
        public string PasswordConfirm { get; set; }
    }
}