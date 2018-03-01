namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;

    [Serializable]
    public class CreateEmploymentViewModel : EmploymentBaseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameRequired))]
        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknameMaxLength))]
        public string Nickname { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameRequired))]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNameMaxLength))]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.GenderRequired))]
        public int Gender { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DateOfBirthRequired))]
        public DateTime DateOfBirth { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PlaceOfBirthMaxLength))]
        public string PlaceOfBirth { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NationalityRequired))]
        public int Nationality { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NRICNoRequired))]
        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NRICNoMaxLength))]
        public string NRICNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.MaritalStatusRequired))]
        public int MaritalStatus { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NoOfChildrenRequired))]
        public int NoOfChildren { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.OverseasPhoneNoRequired))]
        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.OverseasPhoneNoMaxLength))]
        public string OverseasPhoneNo { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalPhoneNoMaxLength))]
        public string LocalPhoneNo { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.OtherPhoneNoMaxLength))]
        public string OtherPhoneNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PassportNoRequired))]
        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PassportNoMaxLength))]
        public string PassportNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DateIssueRequired))]
        public DateTime DateIssue { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ExpiredDateRequired))]
        public DateTime ExpiredDate { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PlaceOfIssueMaxLength))]
        public string PlaceOfIssue { get; set; }

        [StringLength(32, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.SkypeMaxLength))]
        public string Skype { get; set; }

        [StringLength(254, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PersonalEmailMaxLength))]
        [EmailAddress(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PersonalEmailInvalid))]
        public string PersonalEmail { get; set; }

        [StringLength(500, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.MailingAddressMaxLength))]
        public string MailingAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PermanentAddressRequired))]
        [StringLength(500, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PermanentAddressMaxLength))]
        public string PermanentAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ResidenceAddressRequired))]
        [StringLength(500, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ResidenceAddressMaxLength))]
        public string ResidenceAddress { get; set; }

        public EmergencyContact EmergencyContact { get; set; } = new EmergencyContact();

        public string ProfilePic { get; set; }

        public string ProfilePicLink
        {
            get
            {
                return string.IsNullOrEmpty(ProfilePic) ? "/assets/styles/images/profile.png" : "/ProfilePicture/" + ProfilePic;
            }
        }

        public IFormFile ProfilePicture { get; set; }
    }
}