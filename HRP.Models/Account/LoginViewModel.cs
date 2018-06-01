namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel
    {
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string RijndaelKey { get; set; }
        public string RijndaelIV { get; set; }
    }
}