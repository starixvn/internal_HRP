namespace HRP.Criteria.Employee
{
    using System;

    public class UpdateEmploymentCriteria : EmploymentBaseCriteria
    {
        public int EmployeeId { get; set; }

        public DateTime? CompanyChangeDate { get; set; }

        public int? CompanyReasonId { get; set; }

        public DateTime? LocalCompanyChangeDate { get; set; }

        public int? LocalCompanyReasonId { get; set; }

        public DateTime? JobTitleChangeDate { get; set; }

        public int? JobTitleReasonId { get; set; }

        public DateTime? LocalJobTitleChangeDate { get; set; }

        public int? LocalJobTitleReasonId { get; set; }

        public DateTime? IDNoChangeDate { get; set; }

        public int? IDNoReasonId { get; set; }

        public DateTime? EmploymentStatusChangeDate { get; set; }

        public DateTime? JobCategoryChangeDate { get; set; }

        public int? JobCategoryReasonId { get; set; }

        public DateTime? JobGradeChangeDate { get; set; }

        public int? JobGradeReasonId { get; set; }

        public DateTime? DivisionChangeDate { get; set; }

        public int? DivisionReasonId { get; set; }

        public DateTime? DepartmentChangeDate { get; set; }

        public int? DepartmentReasonId { get; set; }

        public DateTime? SectionChangeDate { get; set; }

        public int? SectionReasonId { get; set; }

        public DateTime? CostCenterChangeDate { get; set; }

        public int? CostCenterReasonId { get; set; }

        public int? GroupDateReasonId { get; set; }

        public int? CompanyDateReasonId { get; set; }

        public bool IsPDFPasswordChanged { get; set; }

        public int UpdatedBy { get; set; }

        public override bool IsValid() => EmployeeId > 0 && IsValidParams() && UpdatedBy > 0;

        public override string GetSettingKey() => "Employment_Update";
    }
}