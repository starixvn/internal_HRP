namespace HRP.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class Section
    {
        private const int CodeLength = 50;
        private const int NameLength = 200;

        public int SectionId { get; set; }

        [Required]
        [StringLength(CodeLength)]
        public string Code { get; set; }

        [StringLength(NameLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DescriptionMaxLength))]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public int CostCenterId { get; set; }

        public string CostCenterCode { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public string DepartmentCode { get; set; }

        [Required]
        public int DivisionId { get; set; }

        public string DivisionCode { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public string CompanyCode { get; set; }
    }
}