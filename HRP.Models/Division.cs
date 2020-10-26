namespace HRP.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class Division
    {
        private const int CodeLength = 50;
        private const int NameLength = 200;

        public int DivisionId { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [Required]
        [StringLength(CodeLength)]
        public string Code { get; set; }

        [StringLength(NameLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DescriptionMaxLength))]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        public string CompanyCode { get; set; }
    }
}