namespace HRP.Models.DataConfiguration
{
    public class UpdateSimpleItemViewModel
    {
        public int FieldId { get; set; }

        public string FieldName { get; set; }

        public string LinkUpdateItem { get; set; }

        public SimpleItem Item { get; set; }
    }
}