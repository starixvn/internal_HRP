namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class WorkPermit
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CompanyRequired")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "WorkPermitType1Required")]
        public int Type1 { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime StartDateType1 { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime EndDateType1 { get; set; }

        public int? Type2 { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime? StartDateType2 { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime? EndDateType2 { get; set; }
    }
}