namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class UpdateEmploymentViewModel : CreateEmploymentViewModel

    {
        [DataType(DataType.Date)]
        public DateTime? CompanyChangeDate { get; set; }

        public int? CompanyReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LocalCompanyChangeDate { get; set; }

        public int? LocalCompanyReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JobTitleChangeDate { get; set; }

        public int? JobTitleReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LocalJobTitleChangeDate { get; set; }

        public int? LocalJobTitleReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? GroupEndDate { get; set; }

        public int? GroupDateReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CompanyEndDate { get; set; }

        public int? CompanyDateReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? IDNoChangeDate { get; set; }

        public int? IDNoReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EmploymentStatusChangeDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JobCategoryChangeDate { get; set; }

        public int? JobCategoryReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JobGradeChangeDate { get; set; }

        public int? JobGradeReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DivisionChangeDate { get; set; }

        public int? DivisionReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DepartmentChangeDate { get; set; }

        public int? DepartmentReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? SectionChangeDate { get; set; }

        public int? SectionReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CostCenterChangeDate { get; set; }

        public int? CostCenterReasonId { get; set; }
    }
}