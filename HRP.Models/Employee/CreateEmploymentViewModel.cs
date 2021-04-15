namespace HRP.Models.Employee
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;

    [Serializable]
    public class CreateEmploymentViewModel : EmploymentBaseViewModel
    {
        private const int MinLength = 20;
        private const int NickNameLength = 30;
        private const int SkypeLength = 32;
        private const int FullNameLength = 70;
        private const int MediumLength = 100;
        private const int PersonalEmailLength = 254;
        private const int MaxLength = 500;

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameRequired))]
        [StringLength(NickNameLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameMaxLength))]
        public string Nickname { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameRequired))]
        [StringLength(FullNameLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameMaxLength))]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.GenderRequired))]
        public int Gender { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DateOfBirthRequired))]
        public DateTime DateOfBirth { get; set; }

        [StringLength(MediumLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PlaceOfBirthMaxLength))]
        public string PlaceOfBirth { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NationalityRequired))]
        public int Nationality { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NRICNoRequired))]
        [StringLength(NickNameLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NRICNoMaxLength))]
        public string NRICNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.MaritalStatusRequired))]
        public int MaritalStatus { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NoOfChildrenRequired))]
        public int NoOfChildren { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.OverseasPhoneNoRequired))]
        [StringLength(MinLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.OverseasPhoneNoMaxLength))]
        public string OverseasPhoneNo { get; set; }

        [StringLength(MinLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalPhoneNoMaxLength))]
        public string LocalPhoneNo { get; set; }

        [StringLength(MinLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.OtherPhoneNoMaxLength))]
        public string OtherPhoneNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PassportNoRequired))]
        [StringLength(MinLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PassportNoMaxLength))]
        public string PassportNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DateIssueRequired))]
        public DateTime DateIssue { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ExpiredDateRequired))]
        public DateTime ExpiredDate { get; set; }

        [StringLength(MediumLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PlaceOfIssueMaxLength))]
        public string PlaceOfIssue { get; set; }

        [StringLength(SkypeLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.SkypeMaxLength))]
        public string Skype { get; set; }

        [StringLength(PersonalEmailLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PersonalEmailMaxLength))]
        [EmailAddress(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PersonalEmailInvalid))]
        public string PersonalEmail { get; set; }

        [StringLength(MaxLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.MailingAddressMaxLength))]
        public string MailingAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PermanentAddressRequired))]
        [StringLength(MaxLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PermanentAddressMaxLength))]
        public string PermanentAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ResidenceAddressRequired))]
        [StringLength(MaxLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ResidenceAddressMaxLength))]
        public string ResidenceAddress { get; set; }

        public EmergencyContact EmergencyContact { get; set; } = new EmergencyContact();

        public IFormFile ProfilePicture { get; set; }

        public CachingFilterViewModel CachingFilter { get; set; }
    }
}