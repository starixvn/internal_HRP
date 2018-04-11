namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Core.Enumerations;

    [Serializable]
    public class WorkPermit
    {
        public int? LocalCompanyId { get; set; }

        public int? Type1 { get; set; }

        [DataType(DataType.Date)]
        [GenericCompare(
            CompareToPropertyName = "EndDateType1",
            DisplayName = "Start Date Type1",
            OtherPropertyDisplayName = "End Date Type1",
            Operator = GenericCompareOperator.LessThanOrEqual)]
        public DateTime? StartDateType1 { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDateType1 { get; set; }

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