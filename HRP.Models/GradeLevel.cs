namespace HRP.Models
{
    using System;

    [Serializable]
    public class GradeLevel
    {
        public int GradeLevelId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}