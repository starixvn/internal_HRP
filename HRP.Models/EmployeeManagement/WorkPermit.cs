namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Core.Enumerations;
    using HRP.Resources;

    [Serializable]
    public class WorkPermit
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CompanyRequired")]
        public int LocalCompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "WorkPermitType1Required")]
        public int Type1 { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "StartDateType1Required")]
        [GenericCompare(
            CompareToPropertyName = "EndDateType1",
            DisplayName = "Start Date Type1",
            OtherPropertyDisplayName = "End Date Type1",
            Operator = GenericCompareOperator.LessThanOrEqual)]
        public DateTime StartDateType1 { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EndDateType1Required")]
        public DateTime EndDateType1 { get; set; }

        public int? Type2 { get; set; }

        [DataType(DataType.Date)]
        [GenericCompare(
            CompareToPropertyName = "EndDateType2",
            DisplayName = "Start Date Type1",
            OtherPropertyDisplayName = "End Date Type2",
            Operator = GenericCompareOperator.LessThanOrEqual)]
        public DateTime? StartDateType2 { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDateType2 { get; set; }
    }
}