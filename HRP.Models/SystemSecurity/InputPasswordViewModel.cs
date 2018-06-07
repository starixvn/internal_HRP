namespace HRP.Models.SystemSecurity
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class InputPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PasswordRequired))]
        public string Password { get; set; }

        public bool IsHost { get; set; }
        public string RijndaelKey { get; set; }
        public string RijndaelIV { get; set; }
    }
}