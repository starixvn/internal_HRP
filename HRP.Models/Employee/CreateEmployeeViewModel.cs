namespace HRP.Models.Employee
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Core.Enumerations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    [Serializable]
    public class CreateEmployeeViewModel
    {
        private const int MinLength = 20;
        private const int NickNameLength = 30;
        private const int SkypeLength = 32;
        private const int FullNameLength = 70;
        private const int MediumLength = 100;
        private const int PersonalEmailLength = 254;
        private const int MaxLength = 500;

        public int EmployeeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameRequired")]
        [StringLength(FullNameLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameRequired")]
        [StringLength(NickNameLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknameMaxLength")]
        public string Nickname { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "GenderRequired")]
        public int Gender { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DateOfBirthRequired")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(MediumLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PlaceOfBirthMaxLength")]
        public string PlaceOfBirth { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "MaritalStatusRequired")]
        public int MaritalStatus { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NoOfChildrenRequired")]
        public int NoOfChildren { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NationalityRequired")]
        public int Nationality { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NRICNoRequired")]
        [StringLength(MinLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NRICNoMaxLength")]
        public string NRICNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PassportNoRequired")]
        [StringLength(MinLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PassportNoMaxLength")]
        public string PassportNo { get; set; }

        [GenericCompare(
            CompareToPropertyName = nameof(ExpiredDate),
            DisplayName = "Date Issue",
            OtherPropertyDisplayName = "Expired Date",
            Operator = GenericCompareOperator.LessThanOrEqual)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DateIssueRequired")]
        public DateTime DateIssue { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ExpiredDateRequired")]
        public DateTime ExpiredDate { get; set; }

        [StringLength(MediumLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PlaceOfIssueMaxLength")]
        public string PlaceOfIssue { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "OverseasPhoneNoRequired")]
        [StringLength(MinLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "OverseasPhoneNoMaxLength")]
        public string OverseasPhoneNo { get; set; }

        [StringLength(MinLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalPhoneNoMaxLength")]
        public string LocalPhoneNo { get; set; }

        [StringLength(MinLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "OtherPhoneNoMaxLength")]
        public string OtherPhoneNo { get; set; }

        [StringLength(SkypeLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "SkypeMaxLength")]
        public string Skype { get; set; }

        [StringLength(PersonalEmailLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PersonalEmailMaxLength")]
        [EmailAddress(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PersonalEmailInvalid")]
        public string PersonalEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PermanentAddressRequired")]
        [StringLength(MaxLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PermanentAddressMaxLength")]
        public string PermanentAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ResidenceAddressRequired")]
        [StringLength(MaxLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ResidenceAddressMaxLength")]
        public string ResidenceAddress { get; set; }

        [StringLength(MaxLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "MailingAddressMaxLength")]
        public string MailingAddress { get; set; }

        public IFormFile ProfilePicture { get; set; }

        public string ProfilePic { get; set; }

        public string ProfilePicLink
        {
            get
            {
                return string.IsNullOrEmpty(ProfilePic) ? "/assets/styles/images/profile.png" : "/ProfilePicture/" + ProfilePic;
            }
        }

        public EmergencyContact EmergencyContact { get; set; } = new EmergencyContact();

        public bool IsHost { get; set; }

        public SelectList GenderList { get; set; }

        public SelectList NationalityList { get; set; }

        public SelectList MaritalStatusList { get; set; }

        public SelectList NoOfChildrenList { get; set; }

        public CachingFilterViewModel CachingFilter { get; set; }
    }
}