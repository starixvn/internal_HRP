namespace HRP.Models.Employee
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    public class UpdateEmploymentViewModel : EmploymentBaseViewModel
    {
        public int EmployeeId { get; set; }

        public int? OldCompanyId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldCompanyId), NewProperty = nameof(CompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVICompanyChangeDateRequired))]
        public override DateTime? CompanyChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCompanyId), NewProperty = nameof(CompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVICompanyReasonRequired))]
        public override int? CompanyReasonId { get; set; }

        public int? OldLocalCompanyId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldLocalCompanyId), NewProperty = nameof(LocalCompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalCompanyChangeDateRequired))]
        public override DateTime? LocalCompanyChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldLocalCompanyId), NewProperty = nameof(LocalCompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalCompanyChangeDateRequired))]
        public override int? LocalCompanyReasonId { get; set; }

        public string OldJobTitle { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldJobTitle), NewProperty = nameof(JobTitle),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVIJobTitleChangeDateRequired))]
        public override DateTime? JobTitleChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldJobTitle), NewProperty = nameof(JobTitle),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVIJobTitleReasonRequired))]
        public override int? JobTitleReasonId { get; set; }

        public int? OldLocalJobTitleId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldLocalJobTitleId), NewProperty = nameof(LocalJobTitleId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalJobTitleChangeDateRequired))]
        public override DateTime? LocalJobTitleChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldLocalJobTitleId), NewProperty = nameof(LocalJobTitleId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalJobTitleReasonRequired))]
        public override int? LocalJobTitleReasonId { get; set; }

        public DateTime? OldGroupStartDate { get; set; }

        public DateTime? OldGroupEndDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldGroupStartDate), NewProperty = nameof(GroupStartDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.GroupDateReasonRequired))]
        [EmploymentChangeValue(OldProperty = nameof(OldGroupEndDate), NewProperty = nameof(GroupEndDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.GroupDateReasonRequired))]
        public override int? GroupDateReasonId { get; set; }

        public DateTime? OldCompanyStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? OldCompanyEndDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCompanyStartDate), NewProperty = nameof(CompanyStartDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CompanyDateReasonRequired))]
        [EmploymentChangeValue(OldProperty = nameof(OldCompanyEndDate), NewProperty = nameof(CompanyEndDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CompanyDateReasonRequired))]
        public override int? CompanyDateReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeeCodeRequired))]
        [StringLength(15, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeeCodeMaxLength))]
        [Remote("CheckEmployeeCodeExisted", "Employee", AdditionalFields = nameof(EmployeeId) + "," + nameof(CompanyCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeeCodeUnique))]
        public override string EmployeeCode { get; set; }

        public string OldEmployeeCode { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldEmployeeCode), NewProperty = nameof(EmployeeCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.IDNoChangeDateRequired))]
        public override DateTime? IDNoChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldEmployeeCode), NewProperty = nameof(EmployeeCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.IDNoReasonRequired))]
        public override int? IDNoReasonId { get; set; }

        public int? OldEmploymentStatus { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldEmploymentStatus), NewProperty = nameof(EmploymentStatus),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmploymentStatusChangeDateRequired))]
        public override DateTime? EmploymentStatusChangeDate { get; set; }

        public int? OldJobCategoryId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldJobCategoryId), NewProperty = nameof(JobCategoryId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobCategoryChangeDateRequired))]
        public override DateTime? JobCategoryChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldJobCategoryId), NewProperty = nameof(JobCategoryId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobCategoryReasonRequired))]
        public override int? JobCategoryReasonId { get; set; }

        public int? OldJobGradeId { get; set; }

        public int? OldGradeLevelId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldJobGradeId), NewProperty = nameof(JobGradeId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobGradeChangeDateRequired))]
        [EmploymentChangeValue(OldProperty = nameof(OldGradeLevelId), NewProperty = nameof(GradeLevelId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobGradeChangeDateRequired))]
        public override DateTime? JobGradeChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldJobGradeId), NewProperty = nameof(JobGradeId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobGradeReasonRequired))]
        [EmploymentChangeValue(OldProperty = nameof(OldGradeLevelId), NewProperty = nameof(GradeLevelId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobGradeReasonRequired))]
        public override int? JobGradeReasonId { get; set; }

        public int? OldDivisionId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldDivisionId), NewProperty = nameof(DivisionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DivisionChangeDateRequired))]
        public override DateTime? DivisionChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldDivisionId), NewProperty = nameof(DivisionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DivisionReasonRequired))]
        public override int? DivisionReasonId { get; set; }

        public int? OldDepartmentId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldDepartmentId), NewProperty = nameof(DepartmentId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DepartmentChangeDateRequired))]
        public override DateTime? DepartmentChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldDepartmentId), NewProperty = nameof(DepartmentId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DepartmentReasonRequired))]
        public override int? DepartmentReasonId { get; set; }

        public int? OldSectionId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldSectionId), NewProperty = nameof(SectionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.SectionChangeDateRequired))]
        public override DateTime? SectionChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldSectionId), NewProperty = nameof(SectionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.SectionReasonRequired))]
        public override int? SectionReasonId { get; set; }

        public int? OldCostCenterId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldCostCenterId), NewProperty = nameof(CostCenterId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CostCenterChangeDateRequired))]
        public override DateTime? CostCenterChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCostCenterId), NewProperty = nameof(CostCenterId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CostCenterReasonRequired))]
        public override int? CostCenterReasonId { get; set; }

        public string OldPDFPassword { get; set; }

        public bool IsHost { get; set; }

        public SelectList JobGrades { get; set; }

        public SelectList GradeLevels { get; set; }

        public SelectList Cities { get; set; }

        public SelectList Offices { get; set; }

        public SelectList Divisions { get; set; }

        public SelectList Departments { get; set; }

        public SelectList Sections { get; set; }

        public SelectList CostCenters { get; set; }

        public SelectList CompanyChangeReasons { get; set; }

        public SelectList LocalCompanyChangeReasons { get; set; }

        public SelectList JobTitleChangeReasons { get; set; }

        public SelectList IDNoChangeReasons { get; set; }

        public SelectList DateCommencementChangeReasons { get; set; }

        public SelectList JobCategoryChangeReasons { get; set; }

        public SelectList JobGradeChangeReasons { get; set; }

        public SelectList DivisionChangeReasons { get; set; }

        public SelectList DepartmentChangeReasons { get; set; }

        public SelectList CostCenterChangeReasons { get; set; }

        public SelectList SectionChangeReasons { get; set; }

        public CachingFilterViewModel CachingFilter { get; set; }
    }
}