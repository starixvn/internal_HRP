namespace HRP.Models.DataConfiguration
{
    using System.Collections.Generic;

    public class SimpleItemTableViewModel
    {
        public SimpleItemTableViewModel()
        {
            Item = new SimpleItem();
        }

        public string FieldName { get; set; }

        public IEnumerable<SimpleItem> Items { get; set; }

        public string LinkCreateItem { get; set; }

        public string LinkUpdateItem { get; set; }

        public SimpleItem Item { get; set; }
    }
}