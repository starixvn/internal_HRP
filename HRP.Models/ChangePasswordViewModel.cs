namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.Constants;

    public class ChangePasswordViewModel
    {
        private const string regexForPassword = "^(?=.*?[A - Z])(?=.*?[a - z])(?=.*?[0 - 9]).{8,50}$";

        [DataType("Password")]
        [RegularExpression(regexForPassword, ErrorMessage = Constants.PasswordRequiredMessage)]
        public string Password { get; set; }

        [DataType("Password")]
        [Compare("Password", ErrorMessage = Constants.PasswordNotMatch)]
        public string PasswordConfirm { get; set; }
    }
}