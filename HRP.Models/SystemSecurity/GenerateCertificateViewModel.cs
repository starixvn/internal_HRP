namespace HRP.Models.SystemSecurity
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class GenerateCertificateViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PasswordRequired))]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,50}$", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PasswordContainRequired))]
        public string Password { get; set; }

        public bool IsHost { get; set; }

        public bool IsSuccessful { get; set; }

        public string MessageResult { get; set; }
    }
}