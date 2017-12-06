
namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType("Password")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Password doesn't match, try again")]
        [DataType("Password")]
        public string PasswordConfirm { get; set; }
    }
}
