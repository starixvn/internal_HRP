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

        public string PlaceOfBirth { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NationalityRequired")]
        public int Nationality { get; set; }

        public SelectList NationalityList { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NRICNoRequired")]
        public string NRICNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "MaritalStatusRequired")]
        public int MaritalStatus { get; set; }

        public SelectList MaritalStatusList { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NoOfChildrenRequired")]
        public int NoOfChildren { get; set; }

        public SelectList NoOfChildrenList { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "OverseasPhoneNoRequired")]
        public string OverseasPhoneNo { get; set; }

        public string LocalPhoneNo { get; set; }

        public string OtherPhoneNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PassportNoRequired")]
        public string PassportNo { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DateIssueRequired")]
        public DateTime DateIssue { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ExpiredDateRequired")]
        public DateTime ExpiredDate { get; set; }

        public string PlaceOfIssue { get; set; }

        public string Skype { get; set; }

        public string PersonalEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PermanentAddressRequired")]
        public string PermanentAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ResidenceAddressRequired")]
        public string ResidenceAddress { get; set; }

        public string MailingAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECNameRequired")]
        public string ECName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECRelationshipRequired")]
        public string ECRelationship { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECAddressRequired")]
        public string ECAddress { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECTelephoneNoRequired")]
        public string ECTelephoneNo { get; set; }

        public string ECTelephoneNo2 { get; set; }

        public IFormFile ProfilePicture { get; set; }
    }
}