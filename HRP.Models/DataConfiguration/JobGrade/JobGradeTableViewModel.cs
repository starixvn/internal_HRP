namespace HRP.Models.DataConfiguration.JobGrade
{
    using System.Collections.Generic;
    using HRP.Models;

    public class JobGradeTableViewModel
    {
        public IEnumerable<JobGrade> JobGradeList { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }

        public string LinkUpdateItem { get; set; }
    }
}