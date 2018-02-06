namespace HRP.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using HRP.Resources;

    [Serializable]
    public class JobGrade
    {
        public int JobGradeId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200, ErrorMessageResourceType = typeof(Message), ErrorMessageResourceName = "DescriptionMaxLength")]
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