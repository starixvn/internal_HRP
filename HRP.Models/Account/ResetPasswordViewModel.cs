namespace HRP.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using SystemSecurity;
    using Resources;

    public class ResetPasswordViewModel : RijndaelData
    {
        [DataType(DataType.Password)]
        [Display(Name = nameof(ViewLang.NewPassword), ResourceType = typeof(ViewLang))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,50}$", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PasswordContainRequired))]
        public string Password { get; set; }

        [Display(Name = nameof(ViewLang.ConfirmPassword), ResourceType = typeof(ViewLang))]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PasswordNotMatch))]
        public string PasswordConfirm { get; set; }
    }
}