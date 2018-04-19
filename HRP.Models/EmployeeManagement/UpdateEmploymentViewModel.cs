namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Core.Model;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;

    public class UpdateEmploymentViewModel : EmploymentBaseViewModel
    {
        public int EmployeeId { get; set; }

        public int? OldCompanyId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldCompanyId), NewProperty = nameof(CompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVICompanyChangeDateRequired))]
        public DateTime? CompanyChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCompanyId), NewProperty = nameof(CompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVICompanyReasonRequired))]
        public int? CompanyReasonId { get; set; }

        public int? OldLocalCompanyId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldLocalCompanyId), NewProperty = nameof(LocalCompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalCompanyChangeDateRequired))]
        public DateTime? LocalCompanyChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldLocalCompanyId), NewProperty = nameof(LocalCompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalCompanyChangeDateRequired))]
        public int? LocalCompanyReasonId { get; set; }

        public string OldJobTitle { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldJobTitle), NewProperty = nameof(JobTitle),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVIJobTitleChangeDateRequired))]
        public DateTime? JobTitleChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldJobTitle), NewProperty = nameof(JobTitle),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVIJobTitleReasonRequired))]
        public int? JobTitleReasonId { get; set; }

        public int? OldLocalJobTitleId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldLocalJobTitleId), NewProperty = nameof(LocalJobTitleId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalJobTitleChangeDateRequired))]
        public DateTime? LocalJobTitleChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldLocalJobTitleId), NewProperty = nameof(LocalJobTitleId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalJobTitleReasonRequired))]
        public int? LocalJobTitleReasonId { get; set; }

        public DateTime? OldGroupStartDate { get; set; }

        public DateTime? OldGroupEndDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldGroupStartDate), NewProperty = nameof(GroupStartDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.GroupDateReasonRequired))]
        [EmploymentChangeValue(OldProperty = nameof(OldGroupEndDate), NewProperty = nameof(GroupEndDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.GroupDateReasonRequired))]
        public int? GroupDateReasonId { get; set; }

        public DateTime? OldCompanyStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? OldCompanyEndDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCompanyStartDate), NewProperty = nameof(CompanyStartDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CompanyDateReasonRequired))]
        [EmploymentChangeValue(OldProperty = nameof(OldCompanyEndDate), NewProperty = nameof(CompanyEndDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CompanyDateReasonRequired))]
        public int? CompanyDateReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeeCodeRequired))]
        [StringLength(15, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeeCodeMaxLength))]
        [Remote("CheckEmployeeCodeExisted", "Employee", AdditionalFields = nameof(EmployeeId) + "," + nameof(CompanyCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeeCodeUnique))]
        public override string EmployeeCode { get; set; }

        public string OldEmployeeCode { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldEmployeeCode), NewProperty = nameof(EmployeeCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.IDNoChangeDateRequired))]
        public DateTime? IDNoChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldEmployeeCode), NewProperty = nameof(EmployeeCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.IDNoReasonRequired))]
        public int? IDNoReasonId { get; set; }

        public int? OldEmploymentStatus { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldEmploymentStatus), NewProperty = nameof(EmploymentStatus),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmploymentStatusChangeDateRequired))]
        public DateTime? EmploymentStatusChangeDate { get; set; }

        public int? OldJobCategoryId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldJobCategoryId), NewProperty = nameof(JobCategoryId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobCategoryChangeDateRequired))]
        public DateTime? JobCategoryChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldJobCategoryId), NewProperty = nameof(JobCategoryId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobCategoryReasonRequired))]
        public int? JobCategoryReasonId { get; set; }

        public int? OldJobGradeId { get; set; }

        public int? OldGradeLevelId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldJobGradeId), NewProperty = nameof(JobGradeId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobGradeChangeDateRequired))]
        [EmploymentChangeValue(OldProperty = nameof(OldGradeLevelId), NewProperty = nameof(GradeLevelId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobGradeChangeDateRequired))]
        public DateTime? JobGradeChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldJobGradeId), NewProperty = nameof(JobGradeId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobGradeReasonRequired))]
        [EmploymentChangeValue(OldProperty = nameof(OldGradeLevelId), NewProperty = nameof(GradeLevelId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobGradeReasonRequired))]
        public int? JobGradeReasonId { get; set; }

        public int? OldDivisionId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldDivisionId), NewProperty = nameof(DivisionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DivisionChangeDateRequired))]
        public DateTime? DivisionChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldDivisionId), NewProperty = nameof(DivisionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DivisionReasonRequired))]
        public int? DivisionReasonId { get; set; }

        public int? OldDepartmentId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldDepartmentId), NewProperty = nameof(DepartmentId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DepartmentChangeDateRequired))]
        public DateTime? DepartmentChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldDepartmentId), NewProperty = nameof(DepartmentId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DepartmentReasonRequired))]
        public int? DepartmentReasonId { get; set; }

        public int? OldSectionId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldSectionId), NewProperty = nameof(SectionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.SectionChangeDateRequired))]
        public DateTime? SectionChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldSectionId), NewProperty = nameof(SectionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.SectionReasonRequired))]
        public int? SectionReasonId { get; set; }

        public int? OldCostCenterId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldCostCenterId), NewProperty = nameof(CostCenterId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CostCenterChangeDateRequired))]
        public DateTime? CostCenterChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCostCenterId), NewProperty = nameof(CostCenterId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CostCenterReasonRequired))]
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

        public IEnumerable<ReferenceResourceModel> CompanyChangeReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> LocalCompanyChangeReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> JobTitleChangeReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> IDNoChangeReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> DateCommencementChangeReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> JobCategoryChangeReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> JobGradeChangeReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> DivisionChangeReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> DepartmentChangeReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> CostCenterChangeReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> SectionChangeReasons { get; set; } = new List<ReferenceResourceModel>();
    }
}