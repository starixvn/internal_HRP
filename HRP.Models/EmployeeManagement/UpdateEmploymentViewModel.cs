namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Core.Enumerations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;

    public class UpdateEmploymentViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVICompanyRequired")]
        public int CompanyId { get; set; }

        public int? OldCompanyId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldCompanyId), NewProperty = nameof(CompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVICompanyChangeDateRequired")]
        public DateTime? CompanyChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCompanyId), NewProperty = nameof(CompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVICompanyReasonRequired")]
        public int? CompanyReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalCompanyRequired")]
        public int LocalCompanyId { get; set; }

        public int? OldLocalCompanyId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldLocalCompanyId), NewProperty = nameof(LocalCompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalCompanyChangeDateRequired")]
        public DateTime? LocalCompanyChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldLocalCompanyId), NewProperty = nameof(LocalCompanyId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalCompanyChangeDateRequired")]
        public int? LocalCompanyReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVIJobTitleRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVIJobTitleMaxLength")]
        public string JobTitle { get; set; }

        public string OldJobTitle { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldJobTitle), NewProperty = nameof(JobTitle),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVIJobTitleChangeDateRequired")]
        public DateTime? JobTitleChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldJobTitle), NewProperty = nameof(JobTitle),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVIJobTitleReasonRequired")]
        public int? JobTitleReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalJobTitleRequired")]
        public int LocalJobTitleId { get; set; }

        public int? OldLocalJobTitleId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldLocalJobTitleId), NewProperty = nameof(LocalJobTitleId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalJobTitleChangeDateRequired")]
        public DateTime? LocalJobTitleChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldLocalJobTitleId), NewProperty = nameof(LocalJobTitleId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalJobTitleReasonRequired")]
        public int? LocalJobTitleReasonId { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "GroupDateCommencementRequired")]
        [GenericCompare(
            CompareToPropertyName = nameof(GroupEndDate),
            DisplayName = "Group Start Date",
            OtherPropertyDisplayName = "Group End Date",
            Operator = GenericCompareOperator.LessThanOrEqual)]
        public DateTime GroupStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? GroupEndDate { get; set; }

        public DateTime? OldGroupStartDate { get; set; }

        public DateTime? OldGroupEndDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldGroupStartDate), NewProperty = nameof(GroupStartDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "GroupDateReasonRequired")]
        [EmploymentChangeValue(OldProperty = nameof(OldGroupEndDate), NewProperty = nameof(GroupEndDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "GroupDateReasonRequired")]
        public int? GroupDateReasonId { get; set; }

        [DataType(DataType.Date)]
        [GenericCompare(
            CompareToPropertyName = nameof(CompanyEndDate),
            DisplayName = "Company Start Date",
            OtherPropertyDisplayName = "Company End Date",
            Operator = GenericCompareOperator.LessThanOrEqual)]
        public DateTime? CompanyStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CompanyEndDate { get; set; }

        public DateTime? OldCompanyStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? OldCompanyEndDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCompanyStartDate), NewProperty = nameof(CompanyStartDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CompanyDateReasonRequired")]
        [EmploymentChangeValue(OldProperty = nameof(OldCompanyEndDate), NewProperty = nameof(CompanyEndDate),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CompanyDateReasonRequired")]
        public int? CompanyDateReasonId { get; set; }

        public string CompanyCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeeCodeRequired")]
        [StringLength(15, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeeCodeMaxLength")]
        [Remote("CheckEmployeeCodeExisted", "EmployeeManagement", AdditionalFields = nameof(EmployeeId) + "," + nameof(CompanyCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeeCodeUnique")]
        public string EmployeeCode { get; set; }

        public string OldEmployeeCode { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldEmployeeCode), NewProperty = nameof(EmployeeCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "IDNoChangeDateRequired")]
        public DateTime? IDNoChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldEmployeeCode), NewProperty = nameof(EmployeeCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "IDNoReasonRequired")]
        public int? IDNoReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmploymentTypeRequired")]
        public int EmploymentType { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmploymentStatusRequired")]
        public int EmploymentStatus { get; set; }

        public int? OldEmploymentStatus { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldEmploymentStatus), NewProperty = nameof(EmploymentStatus),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmploymentStatusChangeDateRequired")]
        public DateTime? EmploymentStatusChangeDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "JobCategoryRequired")]
        public int JobCategoryId { get; set; }

        public int? OldJobCategoryId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldJobCategoryId), NewProperty = nameof(JobCategoryId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "JobCategoryChangeDateRequired")]
        public DateTime? JobCategoryChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldJobCategoryId), NewProperty = nameof(JobCategoryId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "JobCategoryReasonRequired")]
        public int? JobCategoryReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "GradeRequired")]
        public int JobGradeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "GradeLevelRequired")]
        public int GradeLevelId { get; set; }

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

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DivisionRequired")]
        public int DivisionId { get; set; }

        public int? OldDivisionId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldDivisionId), NewProperty = nameof(DivisionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DivisionChangeDateRequired")]
        public DateTime? DivisionChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldDivisionId), NewProperty = nameof(DivisionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DivisionReasonRequired")]
        public int? DivisionReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DepartmentRequired")]
        public int DepartmentId { get; set; }

        public int? OldDepartmentId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldDepartmentId), NewProperty = nameof(DepartmentId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DepartmentChangeDateRequired")]
        public DateTime? DepartmentChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldDepartmentId), NewProperty = nameof(DepartmentId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DepartmentReasonRequired")]
        public int? DepartmentReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "SectionRequired")]
        public int SectionId { get; set; }

        public int? OldSectionId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldSectionId), NewProperty = nameof(SectionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "SectionChangeDateRequired")]
        public DateTime? SectionChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldSectionId), NewProperty = nameof(SectionId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "SectionReasonRequired")]
        public int? SectionReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CostCenterRequired")]
        public int CostCenterId { get; set; }

        public int? OldCostCenterId { get; set; }

        [DataType(DataType.Date)]
        [EmploymentChangeValue(OldProperty = nameof(OldCostCenterId), NewProperty = nameof(CostCenterId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CostCenterChangeDateRequired")]
        public DateTime? CostCenterChangeDate { get; set; }

        [EmploymentChangeValue(OldProperty = nameof(OldCostCenterId), NewProperty = nameof(CostCenterId),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CostCenterReasonRequired")]
        public int? CostCenterReasonId { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = "EmailAddress")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmailRequired")]
        [StringLength(254, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmailMaxLength")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "InvalidEmail")]
        [EmailAddress(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "InvalidEmail")]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CountryRequired")]
        public int CountryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CityRequired")]
        public int CityId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "OfficeRequired")]
        public int OfficeId { get; set; }

        public int? DST { get; set; }

        public int? DBT { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "TaxIDNoMaxLength")]
        public string TaxIDNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "TaxIDNoMaxLength")]
        public string SSSNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "HealthInsNoMaxLength")]
        public string HealthInsNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "HDMFNoMaxLength")]
        public string HDMFNo { get; set; }

        public WorkPermit WorkPermit { get; set; } = new WorkPermit();

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNamePFileAdminRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNamePFileAdminMaxLength")]
        public string FullNamePFileAdmin { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NicknamePFileAdminMaxLength")]
        public string NicknamePFileAdmin { get; set; }

        public Paymaster Paymaster { get; set; } = new Paymaster();

        public EPA EPA { get; set; } = new EPA();

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PDFPasswordRequired")]
        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "PDFPasswordMaxLength")]
        public string PDFPassword { get; set; }

        public bool IsHost { get; set; }

        public IEnumerable<Company> Companies { get; set; } = new List<Company>();

        public IEnumerable<ReferenceResourceModel> LocalCompanies { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> LocalJobTitles { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> EmploymentTypes { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> EmploymentStatuses { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<JobGrade> JobGrades { get; set; } = new List<JobGrade>();

        public IEnumerable<GradeLevel> GradeLevels { get; set; } = new List<GradeLevel>();

        public IEnumerable<JobCategory> JobCategories { get; set; } = new List<JobCategory>();

        public IEnumerable<Country> Countries { get; set; } = new List<Country>();

        public IEnumerable<City> Cities { get; set; } = new List<City>();

        public IEnumerable<Office> Offices { get; set; } = new List<Office>();

        public IEnumerable<ReferenceResourceModel> WorkPermitType1s { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> WorkPermitType2s { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> CommonReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> GroupDateReasons { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<Division> Divisions { get; set; } = new List<Division>();

        public IEnumerable<Department> Departments { get; set; } = new List<Department>();

        public IEnumerable<Section> Sections { get; set; } = new List<Section>();

        public IEnumerable<CostCenter> CostCenters { get; set; } = new List<CostCenter>();

        public EmployeeManagementViewModel Filter { get; set; } = new EmployeeManagementViewModel();
    }
}