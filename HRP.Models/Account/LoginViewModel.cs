namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;

    public class LoginViewModel : RijndaelData
    {
        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}