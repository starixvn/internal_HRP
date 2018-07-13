namespace HRP.Models.Employee
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Core.CustomValidations;
    using HRP.Core.Enumerations;
    using HRP.Resources;
    using Microsoft.AspNetCore.Mvc;

    public class Employment
    {
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVICompanyRequired))]
        public virtual int CompanyId { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? CompanyChangeDate { get; set; }

        public virtual int? CompanyReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalCompanyRequired))]
        public virtual int LocalCompanyId { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? LocalCompanyChangeDate { get; set; }

        public virtual int? LocalCompanyReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVIJobTitleRequired))]
        [StringLength(100, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.BVIJobTitleMaxLength))]
        public virtual string JobTitle { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? JobTitleChangeDate { get; set; }

        public virtual int? JobTitleReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.LocalJobTitleRequired))]
        public virtual int LocalJobTitleId { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? LocalJobTitleChangeDate { get; set; }

        public virtual int? LocalJobTitleReasonId { get; set; }

        [DataType(DataType.Date)]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.GroupDateCommencementRequired))]
        [GenericCompare(
            CompareToPropertyName = nameof(GroupEndDate),
            DisplayName = "Group Start Date",
            OtherPropertyDisplayName = "Group End Date",
            Operator = GenericCompareOperator.LessThanOrEqual)]
        public virtual DateTime GroupStartDate { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? GroupEndDate { get; set; }

        public virtual int? GroupDateReasonId { get; set; }

        [DataType(DataType.Date)]
        [GenericCompare(
            CompareToPropertyName = nameof(CompanyEndDate),
            DisplayName = "Company Start Date",
            OtherPropertyDisplayName = "Company End Date",
            Operator = GenericCompareOperator.LessThanOrEqual)]
        public virtual DateTime? CompanyStartDate { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? CompanyEndDate { get; set; }

        public virtual int? CompanyDateReasonId { get; set; }

        public virtual string CompanyCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeeCodeRequired))]
        [StringLength(15, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeeCodeMaxLength))]
        [Remote("CheckEmployeeCodeExisted", "Employee", AdditionalFields = nameof(CompanyCode),
            ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmployeeCodeUnique))]
        public virtual string EmployeeCode { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? IDNoChangeDate { get; set; }

        public virtual int? IDNoReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmploymentTypeRequired))]
        public virtual int EmploymentType { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmploymentStatusRequired))]
        public virtual int EmploymentStatus { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? EmploymentStatusChangeDate { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.JobCategoryRequired))]
        public virtual int JobCategoryId { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? JobCategoryChangeDate { get; set; }

        public virtual int? JobCategoryReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.GradeRequired))]
        public virtual int JobGradeId { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? JobGradeChangeDate { get; set; }

        public virtual int? JobGradeReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.GradeLevelRequired))]
        public virtual int GradeLevelId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DivisionRequired))]
        public virtual int DivisionId { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? DivisionChangeDate { get; set; }

        public virtual int? DivisionReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DepartmentRequired))]
        public virtual int DepartmentId { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? DepartmentChangeDate { get; set; }

        public virtual int? DepartmentReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.SectionRequired))]
        public virtual int SectionId { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? SectionChangeDate { get; set; }

        public virtual int? SectionReasonId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CostCenterRequired))]
        public virtual int CostCenterId { get; set; }

        [DataType(DataType.Date)]
        public virtual DateTime? CostCenterChangeDate { get; set; }

        public virtual int? CostCenterReasonId { get; set; }

        [Display(ResourceType = typeof(ViewLang), Name = nameof(ViewLang.EmailAddress))]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmailRequired))]
        [StringLength(254, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.EmailMaxLength))]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.InvalidEmail))]
        [EmailAddress(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.InvalidEmail))]
        public virtual string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CountryRequired))]
        public virtual int CountryId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.CityRequired))]
        public virtual int CityId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.OfficeRequired))]
        public virtual int OfficeId { get; set; }

        public virtual int? DST { get; set; }

        public virtual int? DBT { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.TaxIDNoMaxLength))]
        public virtual string TaxIDNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.TaxIDNoMaxLength))]
        public virtual string SSSNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.HealthInsNoMaxLength))]
        public virtual string HealthInsNo { get; set; }

        [StringLength(50, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.HDMFNoMaxLength))]
        public virtual string HDMFNo { get; set; }

        public WorkPermit WorkPermit { get; set; } = new WorkPermit();

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNamePFileAdminRequired))]
        [StringLength(70, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.FullNamePFileAdminMaxLength))]
        public virtual string FullNamePFileAdmin { get; set; }

        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.NicknamePFileAdminMaxLength))]
        public virtual string NicknamePFileAdmin { get; set; }

        public Paymaster Paymaster { get; set; } = new Paymaster();

        public EPA EPA { get; set; } = new EPA();

        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PDFPasswordRequired))]
        [StringLength(30, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.PDFPasswordMaxLength))]
        public virtual string PDFPassword { get; set; }
    }
}