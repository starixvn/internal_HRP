namespace HRP.Models.EmployeeManagement
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class CreateEmploymentViewModel
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVICompanyRequired")]
        public int BVICompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalCompanyRequired")]
        public int LocalCompanyId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVIJobTitleRequired")]
        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "BVIJobTitleMaxLength")]
        public string BVIJobTitle { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "LocalJobTitleRequired")]
        public int LocalJobTitleId { get; set; }

        public DateTime GroupStartDate { get; set; }

        public DateTime GroupEndDate { get; set; }

        public DateTime CompanyStartDate { get; set; }

        public DateTime CompanyEndDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "CompanyCodeRequired")]
        public string CompanyCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeeCodeRequired")]
        [StringLength(15, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmployeeCodeMaxLength")]
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

        public int DST { get; set; }

        public int DBT { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "TaxIDNoMaxLength")]
        public string TaxIDNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "TaxIDNoMaxLength")]
        public string SSSNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "HealthInsNoMaxLength")]
        public string HealthInsNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "HDMFNoMaxLength")]
        public string HDMFNo { get; set; }

        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "FullNameMaxLength")]
        public string FullNamePFileAdmin { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "NickNameMaxLength")]
        public string NickNamePFileAdmin { get; set; }

        public string PDFPassword { get; set; }

        public IEnumerable<Company> BVICompanies { get; set; } = new List<Company>();

        public IEnumerable<Company> LocalCompanies { get; set; } = new List<Company>();

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
    }
}