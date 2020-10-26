namespace HRP.Models
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;

    public class CreateUserViewModel
    {
        private const int FullNameLength = 70;
        private const int PhoneNumberLength = 20;
        private const int EmailLength = 70;

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "UsernameRequired")]
        [RegularExpression(@"^\S{2,30}$", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "UsernameLengthNoSpaceAllowed")]
        [Remote("CheckUsernameExisted", "UserManagement", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "UsernameUnique")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PasswordRequired")]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,50}$", ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PasswordContainRequired")]
        public string Password { get; set; }

        [StringLength(FullNameLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        [Display(ResourceType = typeof(ViewLang), Name = "FullName")]
        public string FullName { get; set; }

        [StringLength(PhoneNumberLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PhoneNumberMaxLength")]
        [Display(ResourceType = typeof(ViewLang), Name = "PhoneNo")]
        [DataType(DataType.PhoneNumber, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "InvalidPhoneNumber")]
        public string PhoneNumber { get; set; }

        [StringLength(EmailLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmailMaxLength")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "InvalidEmail")]
        [Display(ResourceType = typeof(ViewLang), Name = "EmailAddress")]
        [EmailAddress(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "InvalidEmail")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "Status")]
        public bool IsActive { get; set; }
    }
}