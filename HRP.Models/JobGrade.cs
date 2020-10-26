namespace HRP.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class JobGrade
    {
        private const int NameLength = 50;
        private const int DescriptionLength = 200;

        public int JobGradeId { get; set; }

        [Required]
        [StringLength(NameLength)]
        public string Name { get; set; }

        [StringLength(DescriptionLength, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = nameof(Message.DescriptionMaxLength))]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public int GradeLevelId { get; set; }

        public string GradeLevelName { get; set; }

        [Required]
        public int JobCategoryId { get; set; }

        public string JobCategoryName { get; set; }
    }
}