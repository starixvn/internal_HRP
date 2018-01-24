namespace HRP.Models.DataConfiguration
{
    using System.Collections.Generic;

    public class SimpleItemTableViewModel
    {
        public SimpleItemTableViewModel()
        {
            NewItem = new SimpleItem();
        }

        public string FieldName { get; set; }

        public IEnumerable<SimpleItem> Items { get; set; }

        public string LinkCreateItem { get; set; }

        public SimpleItem NewItem { get; set; }
    }
}