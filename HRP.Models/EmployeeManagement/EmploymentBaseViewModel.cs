namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Core.Enumerations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;

    public class EmploymentBaseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVICompanyRequired))]
        public int CompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalCompanyRequired))]
        public int LocalCompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVIJobTitleRequired))]
        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVIJobTitleMaxLength))]
        public string JobTitle { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalJobTitleRequired))]
        public int LocalJobTitleId { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.GroupDateCommencementRequired))]
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

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeeCodeRequired))]
        [StringLength(15, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeeCodeMaxLength))]
        [Remote("CheckEmployeeCodeExisted", "Employee", AdditionalFields = nameof(CompanyCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeeCodeUnique))]
        public virtual string EmployeeCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmploymentTypeRequired))]
        public int EmploymentType { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmploymentStatusRequired))]
        public int EmploymentStatus { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobCategoryRequired))]
        public int JobCategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.GradeRequired))]
        public int JobGradeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.GradeLevelRequired))]
        public int GradeLevelId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DivisionRequired))]
        public int DivisionId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DepartmentRequired))]
        public int DepartmentId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.SectionRequired))]
        public int SectionId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CostCenterRequired))]
        public int CostCenterId { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.EmailAddress))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmailRequired))]
        [StringLength(254, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmailMaxLength))]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.InvalidEmail))]
        [EmailAddress(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.InvalidEmail))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CountryRequired))]
        public int CountryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CityRequired))]
        public int CityId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.OfficeRequired))]
        public int OfficeId { get; set; }

        public int? DST { get; set; }

        public int? DBT { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.TaxIDNoMaxLength))]
        public string TaxIDNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.TaxIDNoMaxLength))]
        public string SSSNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.HealthInsNoMaxLength))]
        public string HealthInsNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.HDMFNoMaxLength))]
        public string HDMFNo { get; set; }

        public WorkPermit WorkPermit { get; set; } = new WorkPermit();

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNamePFileAdminRequired))]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNamePFileAdminMaxLength))]
        public string FullNamePFileAdmin { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknamePFileAdminMaxLength))]
        public string NicknamePFileAdmin { get; set; }

        public Paymaster Paymaster { get; set; } = new Paymaster();

        public EPA EPA { get; set; } = new EPA();

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PDFPasswordRequired))]
        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PDFPasswordMaxLength))]
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