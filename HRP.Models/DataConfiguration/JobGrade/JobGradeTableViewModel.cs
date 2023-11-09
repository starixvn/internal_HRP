namespace HRP.Models.DataConfiguration
{
    using System.Collections.Generic;

    public class JobGradeTableViewModel
    {
        public IEnumerable<JobGrade> JobGradeList { get; set; }

        public Field Field { get; set; }

        public bool IsHost { get; set; }
    }
}