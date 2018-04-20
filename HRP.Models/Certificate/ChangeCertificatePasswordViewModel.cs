namespace HRP.Models.Certificate
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class ChangeCertificatePasswordViewModel : GenerateCertificateViewModel
    {
        [Display(Name = nameof(ViewLang.OldPassword), ResourceType = typeof(ViewLang))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public string OldPassword { get; set; }
    }
}