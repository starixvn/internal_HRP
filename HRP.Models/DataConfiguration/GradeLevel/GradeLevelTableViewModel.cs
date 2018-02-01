namespace HRP.Models.DataConfiguration.GradeLevel
{
    using System.Collections.Generic;
    using HRP.Models;

    public class GradeLevelTableViewModel
    {
        public IEnumerable<GradeLevel> GradeLevelList { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }

        public string LinkUpdateItem { get; set; }
    }
}