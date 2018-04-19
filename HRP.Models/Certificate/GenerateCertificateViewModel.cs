namespace HRP.Models.Certificate
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class GenerateCertificateViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PasswordRequired))]
        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        public string Password { get; set; }

        public bool IsSystemAdmin { get; set; }

        public bool IsIsSuccessful { get; set; }

        public string MessageResult { get; set; }
    }
}