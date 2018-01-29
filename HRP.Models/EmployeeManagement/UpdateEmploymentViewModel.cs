namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;

    public class UpdateEmploymentViewModel : EmploymentBaseViewModel
    {
        public int EmployeeId { get; set; }

        public int? OldCompanyId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldCompanyId), NewProperty = nameof(CompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVICompanyChangeDateRequired")]
        public DateTime? CompanyChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCompanyId), NewProperty = nameof(CompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVICompanyReasonRequired")]
        public int? CompanyReasonId { get; set; }

        public int? OldLocalCompanyId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldLocalCompanyId), NewProperty = nameof(LocalCompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalCompanyChangeDateRequired")]
        public DateTime? LocalCompanyChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldLocalCompanyId), NewProperty = nameof(LocalCompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalCompanyChangeDateRequired")]
        public int? LocalCompanyReasonId { get; set; }

        public string OldJobTitle { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldJobTitle), NewProperty = nameof(JobTitle),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVIJobTitleChangeDateRequired")]
        public DateTime? JobTitleChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldJobTitle), NewProperty = nameof(JobTitle),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVIJobTitleReasonRequired")]
        public int? JobTitleReasonId { get; set; }

        public int? OldLocalJobTitleId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldLocalJobTitleId), NewProperty = nameof(LocalJobTitleId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalJobTitleChangeDateRequired")]
        public DateTime? LocalJobTitleChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldLocalJobTitleId), NewProperty = nameof(LocalJobTitleId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalJobTitleReasonRequired")]
        public int? LocalJobTitleReasonId { get; set; }

        public DateTime? OldGroupStartDate { get; set; }

        public DateTime? OldGroupEndDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldGroupStartDate), NewProperty = nameof(GroupStartDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "GroupDateReasonRequired")]
        [EmploymentChangeValue(OldProperty = nameof(OldGroupEndDate), NewProperty = nameof(GroupEndDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "GroupDateReasonRequired")]
        public int? GroupDateReasonId { get; set; }

        public DateTime? OldCompanyStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? OldCompanyEndDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCompanyStartDate), NewProperty = nameof(CompanyStartDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CompanyDateReasonRequired")]
        [EmploymentChangeValue(OldProperty = nameof(OldCompanyEndDate), NewProperty = nameof(CompanyEndDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CompanyDateReasonRequired")]
        public int? CompanyDateReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeeCodeRequired")]
        [StringLength(15, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeeCodeMaxLength")]
        [Remote("CheckEmployeeCodeExisted", "EmployeeManagement", AdditionalFields = nameof(EmployeeId) + "," + nameof(CompanyCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeeCodeUnique")]
        public override string EmployeeCode { get; set; }

        public string OldEmployeeCode { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldEmployeeCode), NewProperty = nameof(EmployeeCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "IDNoChangeDateRequired")]
        public DateTime? IDNoChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldEmployeeCode), NewProperty = nameof(EmployeeCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "IDNoReasonRequired")]
        public int? IDNoReasonId { get; set; }

        public int? OldEmploymentStatus { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldEmploymentStatus), NewProperty = nameof(EmploymentStatus),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmploymentStatusChangeDateRequired")]
        public DateTime? EmploymentStatusChangeDate { get; set; }

        public int? OldJobCategoryId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldJobCategoryId), NewProperty = nameof(JobCategoryId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "JobCategoryChangeDateRequired")]
        public DateTime? JobCategoryChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldJobCategoryId), NewProperty = nameof(JobCategoryId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "JobCategoryReasonRequired")]
        public int? JobCategoryReasonId { get; set; }

        public int? OldJobGradeId { get; set; }

        public int? OldGradeLevelId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldJobGradeId), NewProperty = nameof(JobGradeId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "JobGradeChangeDateRequired")]
        [EmploymentChangeValue(OldProperty = nameof(OldGradeLevelId), NewProperty = nameof(GradeLevelId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "JobGradeChangeDateRequired")]
        public DateTime? JobGradeChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldJobGradeId), NewProperty = nameof(JobGradeId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "JobGradeReasonRequired")]
        [EmploymentChangeValue(OldProperty = nameof(OldGradeLevelId), NewProperty = nameof(GradeLevelId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "JobGradeReasonRequired")]
        public int? JobGradeReasonId { get; set; }

        public int? OldDivisionId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldDivisionId), NewProperty = nameof(DivisionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DivisionChangeDateRequired")]
        public DateTime? DivisionChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldDivisionId), NewProperty = nameof(DivisionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DivisionReasonRequired")]
        public int? DivisionReasonId { get; set; }

        public int? OldDepartmentId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldDepartmentId), NewProperty = nameof(DepartmentId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DepartmentChangeDateRequired")]
        public DateTime? DepartmentChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldDepartmentId), NewProperty = nameof(DepartmentId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DepartmentReasonRequired")]
        public int? DepartmentReasonId { get; set; }

        public int? OldSectionId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldSectionId), NewProperty = nameof(SectionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "SectionChangeDateRequired")]
        public DateTime? SectionChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldSectionId), NewProperty = nameof(SectionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "SectionReasonRequired")]
        public int? SectionReasonId { get; set; }

        public int? OldCostCenterId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldCostCenterId), NewProperty = nameof(CostCenterId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CostCenterChangeDateRequired")]
        public DateTime? CostCenterChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCostCenterId), NewProperty = nameof(CostCenterId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CostCenterReasonRequired")]
        public int? CostCenterReasonId { get; set; }

        public string OldPDFPassword { get; set; }

        public bool IsHost { get; set; }

        public IEnumerable<JobGrade> JobGrades { get; set; } = new List<JobGrade>();

        public IEnumerable<GradeLevel> GradeLevels { get; set; } = new List<GradeLevel>();

        public IEnumerable<City> Cities { get; set; } = new List<City>();

        public IEnumerable<Office> Offices { get; set; } = new List<Office>();

        public IEnumerable<Division> Divisions { get; set; } = new List<Division>();

        public IEnumerable<Department> Departments { get; set; } = new List<Department>();

        public IEnumerable<Section> Sections { get; set; } = new List<Section>();

        public IEnumerable<CostCenter> CostCenters { get; set; } = new List<CostCenter>();

        public IEnumerable<ReferenceResourceModel> CommonReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> GroupDateReasons { get; set; } = new List<ReferenceResourceModel>();
    }
}