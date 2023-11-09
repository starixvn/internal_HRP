namespace HRP.Models.DataConfiguration
{
    using System.Collections.Generic;

    public class SectionTableViewModel
    {
        public IEnumerable<Section> Sections { get; set; }

        public Field Field { get; set; }

        public bool IsHost { get; set; }
    }
}