namespace HRP.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string Code { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DescriptionMaxLength")]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public int DivisionId { get; set; }

        public string DivisionCode { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }
    }
}