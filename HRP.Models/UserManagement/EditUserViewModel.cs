namespace HRP.Models.UserManagement
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class EditUserViewModel
    {
        public int UserId { get; set; }

        public string Username { get; set; }

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

        [Display(ResourceType = typeof(Message), Name = "UserIsActiveLabel")]
        public bool IsActive { get; set; }

        public bool IsSystemAdmin { get; set; }
    }
}
