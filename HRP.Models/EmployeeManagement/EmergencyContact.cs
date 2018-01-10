namespace HRP.Models.EmployeeManagement
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class EmergencyContact
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECNameRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECNameMaxLength")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECRelationshipRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECRelationshipMaxLength")]
        public string Relationship { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECAddressRequired")]
        [StringLength(500, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECAddressMaxLength")]
        public string Address { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECTelephoneNoRequired")]
        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECTelephoneNoMaxLength")]
        public string PhoneNo1 { get; set; }

        [StringLength(20, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "ECTelephoneNoMaxLength")]
        public string PhoneNo2 { get; set; }
    }
}