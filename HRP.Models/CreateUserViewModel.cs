namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;

    public class CreateUserViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "UsernameRequired")]
        [RegularExpression(@"^\S{2,30}$", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "UsernameLengthNoSpaceAllowed")]
        [Remote("CheckUsernameExisted", "UserManagement", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "UsernameUnique")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PasswordRequired")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,50}$", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PasswordContainRequired")]
        public string Password { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        [Display(ResourceType = typeof(ViewLang), Name = "FullName")]
        public string FullName { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PhoneNumberMaxLength")]
        [Display(ResourceType = typeof(ViewLang), Name = "PhoneNo")]
        [DataType(DataType.PhoneNumber, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "InvalidPhoneNumber")]
        public string PhoneNumber { get; set; }

        [StringLength(254, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmailMaxLength")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "InvalidEmail")]
        [Display(ResourceType = typeof(ViewLang), Name = "EmailAddress")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "Status")]
        public bool IsActive { get; set; }
    }
}