namespace HRP.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,50}$", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PasswordContainRequired))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PasswordNotMatch))]
        public string PasswordConfirm { get; set; }
    }
}