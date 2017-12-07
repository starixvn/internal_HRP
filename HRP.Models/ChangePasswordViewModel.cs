namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ChangePasswordViewModel
    {
        private const string regexForPassword = "^(?=.*[a-z])(?=.*[A-Z])(?=.*[0-9]).{8,50}$";

        private const string passwordRequired = "Password must contain at least 8 characters, including UPPER/lowercase and number.";

        private const string passwordNotMatch = "Password doesn't match, try again";

        [Required]
        [DataType("Password")]
        [RegularExpression(regexForPassword, ErrorMessage = passwordRequired)]
        public string Password { get; set; }

        [Required]
        [DataType("Password")]
        [Compare("Password", ErrorMessage = passwordNotMatch)]
        [Display(Name = "Password confirm")]
        public string PasswordConfirm { get; set; }
    }
}