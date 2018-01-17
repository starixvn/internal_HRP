namespace HRP.Models
{
    using System;

    [Serializable]
    public class JobGrade
    {
        public int JobGradeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }
    }
}