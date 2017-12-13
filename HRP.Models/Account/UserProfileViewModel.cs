namespace HRP.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class UserProfileViewModel
    {
        public string FullName { get; set; }

        [StringLength(20, ErrorMessage = "Phone number must not over 20 characters long")]
        public string Phone { get; set; }

        [StringLength(254, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmailMaxLength")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmailRequired")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}