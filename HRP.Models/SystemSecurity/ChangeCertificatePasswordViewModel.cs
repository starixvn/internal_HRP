namespace HRP.Models.SystemSecurity
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class ChangeCertificatePasswordViewModel : GenerateCertificateViewModel
    {
        [Display(Name = nameof(ViewLang.OldPassword), ResourceType = typeof(ViewLang))]
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PasswordNotMatch))]
        public string ConfirmPassword { get; set; }
    }
}