namespace HRP.Models.Account
{
    using System.ComponentModel.DataAnnotations;
    using Resources;

    public class UserProfileViewModel
    {
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.FullName))]
        public string FullName { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.PhoneNo))]
        [DataType(DataType.PhoneNumber, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.InvalidPhoneNumber))]
        public string Phone { get; set; }

        [StringLength(254, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.StringLengthMaxErrorMessage))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.RequiredErrorMessage))]
        [DataType(DataType.EmailAddress)]
        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.EmailAddress))]
        [EmailAddress(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.InvalidEmail))]
        public string Email { get; set; }
    }
}