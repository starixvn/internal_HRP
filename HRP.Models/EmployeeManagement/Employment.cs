namespace HRP.Models.EmployeeManagement
{
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    public class Employment
    {
        public int EmployeeId { get; set; }

        public int BVICompanyId { get; set; }

        public int LocalCompanyId { get; set; }

        [StringLength(50)]
        public string BVIJobTitle { get; set; }

        public int LocalJobTitleId { get; set; }

        public string CompanyCode { get; set; }

        [StringLength(15)]
        public string EmployeeCode { get; set; }

        public int EmploymentType { get; set; }

        public int EmploymentStatus { get; set; }

        public int JobCategoryId { get; set; }

        public int GradeId { get; set; }

        public int GradeLevelId { get; set; }

        public int DivisionId { get; set; }

        public int DepartmentId { get; set; }

        public int SectionId { get; set; }

        public int CostCenterId { get; set; }

        [StringLength(254, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmailMaxLength")]
        [Required(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "EmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "InvalidEmail")]
        [Display(ResourceType = typeof(ViewLang), Name = "EmailAddress")]
        [EmailAddress(ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "InvalidEmail")]
        public string Email { get; set; }

        public int Country { get; set; }

        public int City { get; set; }

        public int Office { get; set; }

        public int DST { get; set; }

        public int DBT { get; set; }

        [StringLength(50)]
        public string TaxIDNo { get; set; }

        [StringLength(50)]
        public string SSSNo { get; set; }

        [StringLength(50)]
        public string HealthInsNo { get; set; }

        [StringLength(50)]
        public string HDMFNo { get; set; }

        [StringLength(70)]
        public string FullNamePFileAdmin { get; set; }

        [StringLength(30)]
        public string NickNamePFileAdmin { get; set; }

        public string PDFPassword { get; set; }
    }
}