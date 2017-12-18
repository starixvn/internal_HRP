namespace HRP.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class UserProfileViewModel
    {
        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        [Display(ResourceType = typeof(Message), Name = "FullNameLabel")]
        public string FullName { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PhoneNumberMaxLength")]
        [Display(ResourceType = typeof(Message), Name = "PhoneNumberLabel")]
        [DataType(DataType.PhoneNumber)]
        [Phone]
        public string Phone { get; set; }

        [StringLength(254, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmailMaxLength")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmailRequired")]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(Message), Name = "EmailLabel")]
        [EmailAddress]
        public string Email { get; set; }
    }
}