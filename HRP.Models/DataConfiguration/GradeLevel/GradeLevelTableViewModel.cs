namespace HRP.Models.DataConfiguration
{
    using System.Collections.Generic;

    public class GradeLevelTableViewModel
    {
        public IEnumerable<GradeLevel> GradeLevelList { get; set; }

        public Field Field { get; set; }

        public bool IsHost { get; set; }
    }
}