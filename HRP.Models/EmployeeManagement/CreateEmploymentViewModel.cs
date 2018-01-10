namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;

    public class CreateEmploymentViewModel
    {
        public int EmployeeId { get; set; }

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
        public DateTime GroupStartDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime? CompanyStartDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CompanyCodeRequired")]
        [Remote("CheckEmployeeCodeExisted", "EmployeeManagement", AdditionalFields = nameof(EmployeeCode), ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeeCodeUnique")]
        public string CompanyCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeeCodeRequired")]
        [StringLength(15, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeeCodeMaxLength")]
        [Remote("CheckEmployeeCodeExisted", "EmployeeManagement", AdditionalFields = nameof(CompanyCode), ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeeCodeUnique")]
        public string EmployeeCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmploymentTypeRequired")]
        public int EmploymentType { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmploymentStatusRequired")]
        public int EmploymentStatus { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "JobCategoryRequired")]
        public int JobCategoryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "GradeRequired")]
        public int GradeId { get; set; }

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

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameRequired")]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNamePFileAdmin { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNamePFileAdmin { get; set; }

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

        public IEnumerable<ReferenceResourceModel> Grades { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> GradeLevels { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> JobCategories { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> Countries { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> Cities { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> Offices { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> TaxDeclarations { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> WorkPermitType1s { get; set; } = new List<ReferenceResourceModel>();

        public IEnumerable<ReferenceResourceModel> WorkPermitType2s { get; set; } = new List<ReferenceResourceModel>();
    }
}