namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class WorkPermit
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CompanyRequired")]
        public int LocalCompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "WorkPermitType1Required")]
        public int Type1 { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime StartDateType1 { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime EndDateType1 { get; set; }

        public int? Type2 { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? StartDateType2 { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? EndDateType2 { get; set; }
    }
}