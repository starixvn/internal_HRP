namespace HRP.Models.DataConfiguration.GradeLevel
{
    using System.Collections.Generic;
    using HRP.Models;

    public class GradeLevelTableViewModel
    {
        public IEnumerable<GradeLevel> GradeLevelList { get; set; }

        public Field Field { get; set; }
    }
}