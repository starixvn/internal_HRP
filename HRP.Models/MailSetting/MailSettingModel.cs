namespace HRP.Models.MailSetting
{
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public class MailSettingModel : Company
    {
        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.SMTP))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [StringLength(
            150,
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        public string Server { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public int? Port { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.SenderEmail))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [StringLength(
            100,
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        public string Username { get; set; }

        [StringLength(
            100,
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        public string Password { get; set; }

        public bool IsAuth { get; set; }
        public int Priority { get; set; }
    }
}