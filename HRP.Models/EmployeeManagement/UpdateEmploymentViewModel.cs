namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Resources;

    public class UpdateEmploymentViewModel : CreateEmploymentViewModel
    {
        public int OldCompanyId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldCompanyId), NewProperty = nameof(CompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVICompanyChangeDateRequired")]
        public DateTime? CompanyChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCompanyId), NewProperty = nameof(CompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVICompanyReasonRequired")]
        public int? CompanyReasonId { get; set; }

        public int OldLocalCompanyId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldLocalCompanyId), NewProperty = nameof(LocalCompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalCompanyChangeDateRequired")]
        public DateTime? LocalCompanyChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldLocalCompanyId), NewProperty = nameof(LocalCompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalCompanyChangeDateRequired")]
        public int? LocalCompanyReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? JobTitleChangeDate { get; set; }

        public int? JobTitleReasonId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? LocalJobTitleChangeDate { get; set; }

        public int? LocalJobTitleReasonId { get; set; }

        public int? GroupDateReasonId { get; set; }

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

        public IEnumerable<ReferenceResourceModel> CommonReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> GroupDateReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<Division> Divisions { get; set; } = new List<Division>();

        public IEnumerable<Department> Departments { get; set; } = new List<Department>();

        public IEnumerable<Section> Sections { get; set; } = new List<Section>();

        public IEnumerable<CostCenter> CostCenters { get; set; } = new List<CostCenter>();
    }
}