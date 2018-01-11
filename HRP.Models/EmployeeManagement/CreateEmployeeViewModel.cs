namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class CreateEmployeeViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "GenderRequired")]
        public int Gender { get; set; }

        public SelectList GenderList { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DateOfBirthRequired")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PlaceOfBirthMaxLength")]
        public string PlaceOfBirth { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NationalityRequired")]
        public int Nationality { get; set; }

        public SelectList NationalityList { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NRICNoRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NRICNoMaxLength")]
        public string NRICNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "MaritalStatusRequired")]
        public int MaritalStatus { get; set; }

        public SelectList MaritalStatusList { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NoOfChildrenRequired")]
        public int NoOfChildren { get; set; }

        public SelectList NoOfChildrenList { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "OverseasPhoneNoRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "OverseasPhoneNoMaxLength")]
        public string OverseasPhoneNo { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalPhoneNoMaxLength")]
        public string LocalPhoneNo { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "OtherPhoneNoMaxLength")]
        public string OtherPhoneNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PassportNoRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PassportNoMaxLength")]
        public string PassportNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DateIssueRequired")]
        public DateTime DateIssue { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ExpiredDateRequired")]
        public DateTime ExpiredDate { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PlaceOfIssueMaxLength")]
        public string PlaceOfIssue { get; set; }

        [StringLength(32, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "SkypeMaxLength")]
        public string Skype { get; set; }

        [StringLength(254, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PersonalEmailMaxLength")]
        [EmailAddress(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PersonalEmailInvalid")]
        public string PersonalEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PermanentAddressRequired")]
        [StringLength(500, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PermanentAddressMaxLength")]
        public string PermanentAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ResidenceAddressRequired")]
        [StringLength(500, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ResidenceAddressMaxLength")]
        public string ResidenceAddress { get; set; }

        [StringLength(500, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "MailingAddressMaxLength")]
        public string MailingAddress { get; set; }

        public EmergencyContact EmergencyContact { get; set; }

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