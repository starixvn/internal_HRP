namespace HRP.Models.DataConfiguration.JobGrade
{
    using System.Collections.Generic;
    using HRP.Models;

    public class JobGradeTableViewModel
    {
        public IEnumerable<JobGrade> JobGradeList { get; set; }

        public Field Field { get; set; }

        public bool IsHost { get; set; }
    }
}