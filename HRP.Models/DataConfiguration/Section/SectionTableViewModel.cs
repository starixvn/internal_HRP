namespace HRP.Models.DataConfiguration.Section
{
    using System.Collections.Generic;
    using HRP.Models;

    public class SectionTableViewModel
    {
        public IEnumerable<Section> Sections { get; set; }

        public Field Field { get; set; }

        public bool IsHost { get; set; }
    }
}