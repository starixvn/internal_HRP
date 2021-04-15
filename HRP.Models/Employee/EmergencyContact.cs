namespace HRP.Models.Employee
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class EmergencyContact
    {
        private const short PhoneNumber_MaxLength = 15;
        private const short MediumLength = 70;
        private const short LargeLength = 500;

        public int EmployeeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECNameRequired))]
        [StringLength(MediumLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECNameMaxLength))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECRelationshipRequired))]
        [StringLength(MediumLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECRelationshipMaxLength))]
        public string Relationship { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECAddressRequired))]
        [StringLength(LargeLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECAddressMaxLength))]
        public string Address { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECTelephoneNoRequired))]
        [StringLength(PhoneNumber_MaxLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECTelephoneNoMaxLength))]
        public string PhoneNo1 { get; set; }

        [StringLength(PhoneNumber_MaxLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECTelephoneNoMaxLength))]
        public string PhoneNo2 { get; set; }
    }
}