namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Core.Enumerations;
    using HRP.Resources;

    public class EmploymentBaseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVICompanyRequired")]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalCompanyRequired")]
        public int LocalCompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVIJobTitleRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVIJobTitleMaxLength")]
        public string JobTitle { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalJobTitleRequired")]
        public int LocalJobTitleId { get; set; }

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

        [DataType(DataType.Date)]
        [GenericCompare(
            CompareToPropertyName = nameof(CompanyEndDate),
            DisplayName = "Company Start Date",
            OtherPropertyDisplayName = "Company End Date",
            Operator = GenericCompareOperator.LessThanOrEqual)]
        public DateTime? CompanyStartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CompanyEndDate { get; set; }

        public string CompanyCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmploymentTypeRequired")]
        public int EmploymentType { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmploymentStatusRequired")]
        public int EmploymentStatus { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "JobCategoryRequired")]
        public int JobCategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "GradeRequired")]
        public int JobGradeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "GradeLevelRequired")]
        public int GradeLevelId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DivisionRequired")]
        public int DivisionId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DepartmentRequired")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "SectionRequired")]
        public int SectionId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CostCenterRequired")]
        public int CostCenterId { get; set; }

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

        public IEnumerable<Company> Companies { get; set; } = new List<Company>();

        public IEnumerable<ReferenceResourceModel> LocalCompanies { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> LocalJobTitles { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> EmploymentTypes { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> EmploymentStatuses { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<JobCategory> JobCategories { get; set; } = new List<JobCategory>();

        public IEnumerable<Country> Countries { get; set; } = new List<Country>();

        public IEnumerable<ReferenceResourceModel> WorkPermitType1s { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> WorkPermitType2s { get; set; } = new List<ReferenceResourceModel>();
    }
}