namespace HRP.Models.Report
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Models.SystemSecurity;
    using HRP.Resources;

    public class BaseSecurityReportViewModel : RijndaelData
    {
        [Display(Name = nameof(ViewLang.Password), ResourceType = typeof(ViewLang))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PasswordRequired))]
        public string FilePassword { get; set; }

        public string EncryptedPassword { get; set; }

        [Display(Name = nameof(ViewLang.ReusePassword), ResourceType = typeof(ViewLang))]
        public bool ReusePassword { get; set; }
    }
}