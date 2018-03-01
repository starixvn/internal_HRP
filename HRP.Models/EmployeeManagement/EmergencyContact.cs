namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class EmergencyContact
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECNameRequired))]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECNameMaxLength))]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECRelationshipRequired))]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECRelationshipMaxLength))]
        public string Relationship { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECAddressRequired))]
        [StringLength(500, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECAddressMaxLength))]
        public string Address { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECTelephoneNoRequired))]
        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECTelephoneNoMaxLength))]
        public string PhoneNo1 { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.ECTelephoneNoMaxLength))]
        public string PhoneNo2 { get; set; }
    }
}