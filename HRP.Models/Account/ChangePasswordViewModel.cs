namespace HRP.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public class ChangePasswordViewModel : ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = nameof(ViewLang.OldPassword), ResourceType = typeof(ViewLang))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public string OldPassword { get; set; }
    }
}