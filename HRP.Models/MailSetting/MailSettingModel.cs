namespace HRP.Models.MailSetting
{
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public class MailSettingModel : Company
    {
        private const int ServerLength = 150;
        private const int NormalLength = 100;

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.SMTP))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [StringLength(
            ServerLength,
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        public string Server { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        public int? Port { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.SenderEmail))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [StringLength(
            NormalLength,
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        public string Username { get; set; }

        [StringLength(
            NormalLength,
            ErrorMessageResourceType = typeof(Message),
            ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        public string Password { get; set; }

        public bool IsAuth { get; set; }
        public int Priority { get; set; }
    }
}