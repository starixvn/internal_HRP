namespace HRP.Models.DataConfiguration
{
    using System.Collections.Generic;

    public class SimpleItemTableViewModel
    {
        public IEnumerable<SimpleItem> Items { get; set; }

        public Field Field { get; set; }
    }
}