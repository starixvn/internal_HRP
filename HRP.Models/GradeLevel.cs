namespace HRP.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class GradeLevel
    {
        private const int NameLength = 50;
        private const int DescriptionLength = 200;

        [Required]
        public int GradeLevelId { get; set; }

        [Required]
        [StringLength(NameLength)]
        public string Name { get; set; }

        [StringLength(DescriptionLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DescriptionMaxLength))]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public int JobCategoryId { get; set; }

        public string JobCategoryName { get; set; }
    }
}