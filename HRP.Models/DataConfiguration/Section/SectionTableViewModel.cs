namespace HRP.Models.DataConfiguration.Section
{
    using System.Collections.Generic;
    using HRP.Models;

    public class SectionTableViewModel
    {
        public IEnumerable<Section> Sections { get; set; }

        public string FieldName { get; set; }

        public string LinkCreateItem { get; set; }

        public string LinkUpdateItem { get; set; }
    }
}